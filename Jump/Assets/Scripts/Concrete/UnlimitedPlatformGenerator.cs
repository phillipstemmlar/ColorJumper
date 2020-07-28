using System.Collections.Generic;
using UnityEngine;

public class UnlimitedPlatformGenerator : PlatformGenerator
{
	int platformHeights;

	protected override void Start() {
		base.Start();
		platformIndex = 0;
		calculateHeightValues();

		float n = Mathf.Floor(CameraHeight / blockHeight);
		for (int i = 0; i < n; ++i) {
			float y = (i + 1) * blockHeight;
			print("" + i + ": " + y);
		}
	}

	protected override void onGenerationInterval() {
		//print("" + platformIndex + ": Generation Interval!");
		generateNextPlatform();
	}
	protected override void onGeneratingFinished() {
		print("Finished Generating!");
	}

	protected override void generateNextPlatform() {
		CreatePlatform(transform.position + Vector3.up * randomHeight());
	}

	protected override Color[] createColorsArray() {
		List<Color> cols = new List<Color>();
		cols.Add(Color.green);
		cols.Add(Color.blue);
		cols.Add(Color.red);
		cols.Add(Color.magenta);
		return cols.ToArray();
	}

	void calculateHeightValues() {
		platformHeights = Mathf.FloorToInt(CameraHeight / blockHeight) - 2;
	}

	float randomHeight() {
		return (Random.Range(0, platformHeights) % platformHeights) * blockHeight - CameraHeight / 2 + blockHeight;
	}
}
