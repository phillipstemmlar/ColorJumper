using System.Collections.Generic;
using UnityEngine;

public class UnlimitedPlatformGenerator : PlatformGenerator
{
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

	public override void onPlayerOutOfBounds(bool bottom) {
		if (bottom) {
			//player.setY(CameraHeight / 2 + VerticalOffset);
			//changeBackgroundColor(randomColorIndex());
			restartGeneration();
		}
	}

	float randomHeight() {
		return getHeight(Random.Range(0, platformHeightsCount));

	}

}
