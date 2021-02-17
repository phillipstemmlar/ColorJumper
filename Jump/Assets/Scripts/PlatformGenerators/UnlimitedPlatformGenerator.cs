using System.Collections.Generic;
using UnityEngine;

public class UnlimitedPlatformGenerator : PlatformGenerator
{

	public float DoublePlatformMinimumDistance = 4f;
	public float DoublePlatformMaximumDistance = 6f;
	public float DoublePlatformChance = 0.5f;
	public float DoublePlatformOpenPercentage = 0.7f;
	int debugCounter = 0;
	protected override void init() {
		type = Type.unlimited;
	}

	protected override void onGenerationInterval() {
		generateNextPlatform();
	}
	protected override void onGeneratingFinished() {
		print("Finished Generating!");
	}

	protected override void generateNextPlatform() {

		float H = randomHeight();
		CreatePlatform(transform.position + Vector3.up * H);



		if (Mathf.Abs(CameraHeight - H) / CameraHeight > DoublePlatformOpenPercentage && Random.value <= DoublePlatformChance) {
			float H2, x;
			do {
				H2 = randomHeight();
				x = Mathf.Abs(H2 - H);
			} while (x < DoublePlatformMinimumDistance || x > DoublePlatformMaximumDistance);

			Debug.Log(debugCounter + " - Diff: " + x + "    -     D: " + DoublePlatformMinimumDistance + " : " + DoublePlatformMaximumDistance);
			debugCounter++;

			CreatePlatform(transform.position + Vector3.up * H2);
		}

	}

	protected override Color[] createColorsArray() {
		List<Color> cols = new List<Color>();
		cols.Add(decCol(60, 174, 163));       //  #3caea3	rgb(60, 174, 163)
		cols.Add(decCol(32, 99, 155));        //  #20639b	rgb(32, 99, 155)
		cols.Add(decCol(237, 85, 59));        //  #ed553b	rgb(237, 85, 59)
		cols.Add(decCol(246, 213, 92));       //  #f6d55c	rgb(246, 213, 92)
		return cols.ToArray();                   //	 #173f5f	rgb(23, 63, 95)
	}

	Color decCol(int r, int g, int b, int a = 255) {
		return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
	}

	public override void onPlayerOutOfBounds(bool bottom) {
		//if (bottom) {
		//	player.setY(CameraHeight / 2 + VerticalOffset);
		//	changeBackgroundColor(randomColorIndex());
		//	//restartGeneration();
		//}
	}

	float randomHeight() {
		return getHeight(Random.Range(0, platformHeightsCount));

	}

}
