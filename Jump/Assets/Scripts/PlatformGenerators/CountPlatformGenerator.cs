using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountPlatformGenerator : PlatformGenerator
{
	public int PlatformCount = 5;

	protected override void init() {
		type = Type.count;
		platformIndex = 0;

	}

	protected override void onGenerationInterval() {
		if (platformIndex < PlatformCount) {
			platformIndex++;
			//print("" + platformIndex + ": Generation Interval!");
			generateNextPlatform();
		} else {
			stopGenerating();
		}
	}
	protected override void onGeneratingFinished() {
		print("Finished Generating!");
	}

	protected override void generateNextPlatform() {
		CreatePlatform(new Vector3(transform.position.x, blockHeight - CameraHeight / 2));
	}

	public override void onPlayerOutOfBounds(bool bottom) {
		print("Before Restarting - count");
		//Vector3 pos = player.transform.position;
		//pos.y = CameraHeight / 2 + VerticalOffset;
		//player.transform.position = pos;
	}

	protected override Color[] createColorsArray() {
		List<Color> cols = new List<Color>();
		cols.Add(Color.green);
		cols.Add(Color.blue);
		cols.Add(Color.red);
		cols.Add(Color.magenta);
		return cols.ToArray();
	}
}
