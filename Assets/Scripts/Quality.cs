using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Quality {
	public enum QualityGrade {
		Junk,
		Brittle,
		Passable,
		Sturdy,
		Magical,
		Mystic
	}

	// Grade values for the first level of shop.
	public static float[] gradeValues1 = {
		0f,
		.20f,
		.85f,
		.95f
	};

	// Grade values for the second level of shop.
	public static float[] gradeValues2 = {
		0f,
		.20f,
		.75f,
		.85f,
		.95f
	};

	// Grade values for the third level of shop.
	public static float[] gradeValues3 = {
		0f,
		.20f,
		.60f,
		.75f,
		.875f,
		.95f
	};

	public static List<QualityGrade> GetPossibleGrades(int shopLevel) {
		var grades = new List<QualityGrade>();

		int amnt = 0;
		switch (shopLevel) {
			case 1: amnt = gradeValues1.Length; break;
			case 2: amnt = gradeValues2.Length; break;
			case 3: amnt = gradeValues3.Length; break;
		}

		for (int i = amnt-1; i >= 0; i--) {
			grades.Add((QualityGrade)i);
		}

		return grades;
	}

	public static QualityGrade FloatToGrade(float value, int shopLevel) {
		switch (shopLevel) {
			case 1:
				for (int i = Quality.gradeValues1.Length-1; i > 0; i--) {
					if (value >= Quality.gradeValues1[i]) return (QualityGrade)i;
				}
				break;

			case 2:
				for (int i = Quality.gradeValues2.Length-1; i > 0; i--) {
					if (value >= Quality.gradeValues2[i]) return (QualityGrade)i;
				}
				break;

			case 3:
				for (int i = Quality.gradeValues3.Length-1; i > 0; i--) {
					if (value >= Quality.gradeValues3[i]) return (QualityGrade)i;
				}
				break;
		}

		// Value must be < 0.
		return QualityGrade.Junk;
	}

	public static Color GradeToColor(QualityGrade grade) {
		switch (grade) {
			case QualityGrade.Mystic:
				return Color.cyan;
			case QualityGrade.Magical:
				return Color.magenta;
			case QualityGrade.Sturdy:
				return Color.green;
			case QualityGrade.Passable:
				return Color.white;
			case QualityGrade.Brittle:
				return Color.yellow;
			case QualityGrade.Junk:
				return Color.red;
			default:
				return Color.red;
		}
	}

	public static string GradeToString(QualityGrade grade) {
		switch (grade) {
			case QualityGrade.Mystic:
				return "Mystic";
			case QualityGrade.Magical:
				return "Magical";
			case QualityGrade.Sturdy:
				return "Sturdy";
			case QualityGrade.Passable:
				return "Passable";
			case QualityGrade.Brittle:
				return "Brittle";
			case QualityGrade.Junk:
				return "Junk";
			default:
				return "";
		}
	}
}
