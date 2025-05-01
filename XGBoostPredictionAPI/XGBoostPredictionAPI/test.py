import requests

payload = {
    "year": 2025,
    "h": "<b>LICEUL PEDAGOGIC BENEDEK ELEK ODORHEIU SECUIESC</b><br/>Real/Liceal/Zi",
    "sp": "<b>(132) Ştiinţe ale Naturii</b><br/>Limba română"
}

res = requests.post("http://localhost:5000/predict", json=payload)
print("Status code:", res.status_code)
print("Raw response text:", res.text)
