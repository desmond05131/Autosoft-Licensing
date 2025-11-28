using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Autosoft_Licensing.Tests.Helpers
{
    /// <summary>
    /// Test helper that runs a hidden UI host on a dedicated STA thread with a message loop.
    /// Use Invoke/Invoke<T> to run actions on the UI thread and marshal exceptions back to the test thread.
    /// Dispose will close the host and stop the UI thread to avoid leaking window handles.
    /// </summary>
    public class UiTestHost : IDisposable
    {
        private Thread _uiThread;
        private Form _hostForm;
        private readonly ManualResetEventSlim _ready = new ManualResetEventSlim(false);
        private readonly TimeSpan _joinTimeout = TimeSpan.FromSeconds(5);
        private bool _disposed;

        public UiTestHost(string hostFormTitle = "TestHost")
        {
            _uiThread = new Thread(() =>
            {
                try
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    _hostForm = new Form
                    {
                        Text = hostFormTitle,
                        Width = 1200,
                        Height = 800,
                        StartPosition = FormStartPosition.CenterScreen
                    };

                    // Ensure the form's window handle is created on the UI thread BEFORE signalling readiness.
                    // Without this, calling _hostForm.Invoke from the test thread can cause the CLR to create the
                    // handle on the test thread (wrong apartment) which leads to Win32Exception.
                    var _ = _hostForm.Handle;

                    // Signal that the host form has been created and is ready
                    _ready.Set();

                    // Run message loop for this thread; Application.Run will block until form is closed
                    Application.Run(_hostForm);
                }
                catch (ThreadAbortException)
                {
                    // expected during forceful shutdown
                }
                catch (Exception ex)
                {
                    // Surface to console for diagnostics; tests will observe exceptions thrown via Invoke.
                    Console.WriteLine("UI thread exception: " + ex);
                    throw;
                }
            });

            _uiThread.SetApartmentState(ApartmentState.STA);
            _uiThread.IsBackground = true;
            _uiThread.Start();

            // Wait for host to be ready
            if (!_ready.Wait(5000))
            {
                throw new TimeoutException("UI host did not become ready in time.");
            }
        }

        /// <summary>
        /// Show the provided control inside the hidden host form (runs on UI thread).
        /// </summary>
        public void ShowControl(Control control)
        {
            if (control == null) throw new ArgumentNullException(nameof(control));
            EnsureNotDisposed();

            // Use Invoke to perform UI operations synchronously on the UI thread.
            Invoke(() =>
            {
                control.Dock = DockStyle.Fill;
                _hostForm.Controls.Clear();
                _hostForm.Controls.Add(control);

                if (!_hostForm.Visible)
                {
                    // non-modal show so control gets a handle and can create child handles
                    _hostForm.Show();
                }
            });
        }

        /// <summary>
        /// Execute an action on the UI thread and wait for completion. Rethrows UI exceptions on the calling thread.
        /// </summary>
        public void Invoke(Action action, int timeoutMs = 5000)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            EnsureNotDisposed();

            var tcs = new TaskCompletionSource<object>();

            // Use hostForm.Invoke so the call runs synchronously on the UI thread.
            _hostForm.Invoke(new Action(() =>
            {
                try
                {
                    action();
                    tcs.SetResult(null);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));

            if (!tcs.Task.Wait(timeoutMs))
                throw new TimeoutException("Invoke timed out.");

            if (tcs.Task.IsFaulted)
                throw tcs.Task.Exception.InnerException;
        }

        /// <summary>
        /// Execute a function on the UI thread and return the result. Rethrows UI exceptions on the calling thread.
        /// </summary>
        public T Invoke<T>(Func<T> func, int timeoutMs = 5000)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));
            EnsureNotDisposed();

            var tcs = new TaskCompletionSource<T>();

            _hostForm.Invoke(new Action(() =>
            {
                try
                {
                    var result = func();
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));

            if (!tcs.Task.Wait(timeoutMs))
                throw new TimeoutException("Invoke<T> timed out.");

            if (tcs.Task.IsFaulted)
                throw tcs.Task.Exception.InnerException;

            return tcs.Task.Result;
        }

        private void EnsureNotDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(UiTestHost));
            if (_hostForm == null)
                throw new ObjectDisposedException(nameof(UiTestHost));
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            try
            {
                if (_hostForm != null && !_hostForm.IsDisposed)
                {
                    var tcs = new TaskCompletionSource<object>();

                    // Request form close on UI thread (BeginInvoke so we don't deadlock if called from UI thread)
                    _hostForm.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            if (!_hostForm.IsDisposed)
                            {
                                _hostForm.Close(); // causes Application.Run to exit
                            }
                            tcs.SetResult(null);
                        }
                        catch (Exception ex)
                        {
                            tcs.SetException(ex);
                        }
                        finally
                        {
                            // Ensure the thread's message loop exits
                            try { Application.ExitThread(); } catch { }
                        }
                    }));

                    // Wait a short time for the UI thread to end gracefully
                    if (!_uiThread.Join(_joinTimeout))
                    {
                        // Try to abort as a last resort (Thread.Abort may be unsupported on some runtimes)
                        try { _uiThread.Abort(); } catch { /* ignore */ }
                    }

                    // Wait for the BeginInvoke action to complete or timeout
                    try { tcs.Task.Wait(1000); } catch { /* ignore */ }
                }
            }
            finally
            {
                try { _hostForm?.Dispose(); } catch { /* ignore */ }
                _hostForm = null;
                _uiThread = null;
                try { _ready.Dispose(); } catch { /* ignore */ }
            }
        }
    }
}