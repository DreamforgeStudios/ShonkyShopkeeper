using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataTransfer {
	public static string GemType;
    public static Quality.QualityGrade currentQuality;

	public static Personality currentPersonality;
	public static Sprite currentSprite;
    //For sending golems to bidding
    public static int shonkyIndex = -1;

    //For remembering the last camera rotation (only need z rot which is either 8 or 72)
    public static float cameraRot = 8;
}
