import os
import requests
import pandas as pd

years = [2022, 2023, 2024]
counties = [
    "AB", "AR", "AG", "BC", "BH", "BN", "BR", "BT", "BV", "BZ",
    "CS", "CL", "CJ", "CT", "CV", "DB", "DJ", "GL", "GR", "GJ",
    "HR", "HD", "IL", "IS", "IF", "MM", "MH", "MS", "NT", "OT",
    "PH", "SM", "SJ", "SB", "SV", "TR", "TM", "TL", "VS", "VL", "VN", "B"
]

output_dir = "./specializari_output_repartizare_format"
os.makedirs(output_dir, exist_ok=True)

def combine_h(row):
    return f"<b>{row['l']}</b><br/>{row['p']}/{row['n']}/{row['fi']}"

def combine_sp(row):
    return f"<b>({row['c']}) {row['sp']}</b><br/>{row['lp']}"

# Évek szerint haladunk
for year in years:
    all_data = []

    for county in counties:
        url = f"http://static.admitere.edu.ro/{year}/repartizare/{county}/data/specialization"
        try:
            response = requests.get(url)
            if response.status_code == 200:
                data = response.json()
                df = pd.DataFrame(data)
                df["An"] = year
                df["Judet"] = county
                all_data.append(df)
                print(f"✅ {year} {county} letöltve.")
            else:
                print(f"⚠️ {year} {county} -> {response.status_code} nem található.")
        except Exception as e:
            print(f"❌ Hiba {year} {county}: {e}")

    # Ha van adatunk az évre
    if all_data:
        full_year_df = pd.concat(all_data, ignore_index=True)

        full_year_df["h"] = full_year_df.apply(combine_h, axis=1)
        full_year_df["sp"] = full_year_df.apply(combine_sp, axis=1)

        repartizare_df = pd.DataFrame({
            "ja": full_year_df["j"],
            "n": "",
            "jp": "",
            "s": "",
            "sc": "",
            "madm": full_year_df["um"],
            "mev": "",
            "mabs": "",
            "nro": "",
            "nmate": "",
            "lm": "",
            "nlm": "",
            "h": full_year_df["h"],
            "sp": full_year_df["sp"]
        })

        output_file = os.path.join(output_dir, f"repartizare_style_{year}.csv")
        repartizare_df.to_csv(output_file, index=False, encoding="utf-8-sig")
        print(f"💾 Mentve: {output_file}")
    else:
        print(f"⚠️ Nincs adat az évre: {year}")