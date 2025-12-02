# check_variants.py
import json, hashlib, sys, os

INPUT = "license.json"   # adjust path if needed
PREVIEW_HASH = "1dff99099d9f120ddb47223def5d1ae5b1de9ba3a7cb0fe90fff1a8bd2273e35".lower()

def sha256_hex(b):
    return hashlib.sha256(b).hexdigest().lower()

with open(INPUT, "rb") as f:
    raw = f.read()

print("File bytes length:", len(raw))
try:
    obj = json.loads(raw.decode("utf-8"))
except Exception as e:
    print("ERROR: cannot parse JSON:", e)
    sys.exit(1)

def minified_bytes(o, ensure_ascii=False):
    s = json.dumps(o, separators=(',', ':'), ensure_ascii=ensure_ascii)
    return s.encode('utf-8')

def pretty_bytes(o):
    s = json.dumps(o, indent=2, sort_keys=False, ensure_ascii=False)
    return s.encode('utf-8')

def sorted_minified_bytes(o):
    s = json.dumps(o, separators=(',', ':'), sort_keys=True, ensure_ascii=False)
    return s.encode('utf-8')

def remove_nulls(o):
    if isinstance(o, dict):
        return {k: remove_nulls(v) for k, v in o.items() if v is not None}
    if isinstance(o, list):
        return [remove_nulls(x) for x in o]
    return o

candidates = {}

# 1) raw file bytes (what you probably hashed)
candidates['raw_file_bytes'] = sha256_hex(raw)

# 2) pretty printed (UTF-8)
candidates['pretty_utf8'] = sha256_hex(pretty_bytes(obj))

# 3) minified (no spaces)
candidates['minified_utf8'] = sha256_hex(minified_bytes(obj))

# 4) sorted keys + minified
candidates['sorted_minified_utf8'] = sha256_hex(sorted_minified_bytes(obj))

# 5) remove nulls then sorted+minified
obj_no_null = remove_nulls(obj)
candidates['no_nulls_sorted_minified'] = sha256_hex(sorted_minified_bytes(obj_no_null))

# 6) same but ensure_ascii True (escape non-ascii)
candidates['sorted_minified_ensure_ascii'] = sha256_hex(json.dumps(obj_no_null, separators=(',', ':'), sort_keys=True, ensure_ascii=True).encode('utf-8'))

# 7) CRLF variants (LF->CRLF)
def crlf(b):
    return b.replace(b'\n', b'\r\n')
candidates['sorted_minified_crlf'] = sha256_hex(crlf(sorted_minified_bytes(obj_no_null)))

# 8) uppercase hex of any - for info, not needed
# print all
print("\nComputed candidate hashes:")
for name, h in candidates.items():
    match = "  <-- MATCH" if h == PREVIEW_HASH else ""
    print(f"{name:30s} {h}{match}")

if PREVIEW_HASH not in candidates.values():
    print("\nNo match found among candidate canonicalizations.")
    print("If none match, try these additional checks:")
    print("- Check if the preview hash was computed after adding the 'ChecksumSHA256' field (it should NOT).")
    print("- Check encoding used by preview (UTF-16 vs UTF-8).")
    print("- Paste the exact Preview JSON (copied from the app) here and re-run.")
else:
    print("\nFOUND MATCH. The matching canonicalization is the one marked '<-- MATCH'.")
