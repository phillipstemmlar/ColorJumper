using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatPlatformGenerator : PlatformGenerator
{
	protected override void init() {
		type = Type.flat;
	}

	protected override void onGenerationInterval() {
		generateNextPlatform();
	}
	protected override void onGeneratingFinished() {
	}

	protected override void generateNextPlatform() {
		float H = getMiddleHeight();
		CreatePlatform(transform.position + Vector3.up * H, false);
	}

	public override int nextColorIndex() {
		return randomColorIndexNot();
	}

	protected override Color[] createColorsArray() {
		GameObject go = SpriteModelManager.Instance.getActiveColorPalettePrefab();
		ColorPalette palette = go.GetComponent<ColorPalette>();

		Color[] colors = new Color[5];
		colors[0] = palette.Color1;
		colors[1] = palette.Color2;
		colors[2] = palette.Color3;
		colors[3] = palette.Color4;
		colors[4] = palette.Color5;

		return colors;
	}


	public override void onPlayerOutOfBounds(bool bottom) {
		if (bottom) {
			player.setY(CameraHeight / 2 + VerticalOffset);
			changeBackgroundColor(randomColorIndex());
			//restartGeneration();
		}
	}
}
