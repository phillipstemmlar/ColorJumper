using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexedPlatformGenerator : PlatformGenerator
{
	public float[] PlatformHeights;

	protected override void Start() {
		base.Start();
		platformIndex = 0;

	}
	protected override void onGenerationInterval() {
		if (platformIndex < PlatformHeights.Length) {
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
		CreatePlatform(transform.position + Vector3.up * PlatformHeights[platformIndex]);
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
