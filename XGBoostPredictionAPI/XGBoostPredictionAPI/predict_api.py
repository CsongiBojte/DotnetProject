from flask import Flask, request, jsonify
import pandas as pd
import xgboost as xgb
import joblib

# Flask app
app = Flask(__name__)

# Load booster model
model = xgb.XGBRegressor()
model.load_model("xgb_model.json")  # native format

# Load encoder and features
encoder, feature_columns = joblib.load("encoder_and_features.pkl")  # adjust if separate file

# Aggregated admission data
df_last = pd.read_csv("aggregated_last_admissions.csv")

@app.route("/predict", methods=["POST"])
def predict():
    try:
        data = request.get_json()
        new_data = pd.DataFrame([data])

        new_data["avg_madm_school"] = df_last[df_last["h"] == new_data["h"].iloc[0]]["last_madm"].mean()
        new_data["avg_madm_sp"] = df_last[df_last["sp"] == new_data["sp"].iloc[0]]["last_madm"].mean()
        new_data["count_per_school_sp"] = df_last[
            (df_last["h"] == new_data["h"].iloc[0]) &
            (df_last["sp"] == new_data["sp"].iloc[0])
        ]["last_madm"].count()

        # Encode categorical columns
        new_encoded = encoder.transform(new_data)

        # Predict
        prediction = model.predict(new_encoded)[0]
        return jsonify({"predicted_madm": float(round(prediction, 2))})

    except Exception as e:
        return jsonify({"error": str(e)}), 500

if __name__ == "__main__":
    app.run(debug=True, port=5000)
