using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{

	public Player player;

	public Score playerScore;
	public HighScore highScores;

	public Text textBoxScore, textBoxHighScore, textBoxScorePanel, textBoxHighScorePanel;

	void Start() {
		highScores = new HighScore();
		playerScore = new Score(null, this);
	}

	public void init(Player _player, Score oldScore = null) {
		player = _player;


		if (oldScore == null) playerScore = new Score(player, this);
		else playerScore = new Score(oldScore);

		textBoxScore.enabled = true;
		textBoxHighScore.enabled = true;
		textBoxScorePanel.text = "";
		textBoxHighScorePanel.text = "";
	}

	void Update() {
		textBoxScore.text = playerScore.ToString();
		textBoxHighScore.text = "High Score:\n" + highScores.ToString();
	}

	public void RestartLevel() {
		init(player);
	}

	public void ContinueLevel() {
		init(player, playerScore);
	}

	void FinalizeScore(Score newScore) {
		textBoxScore.enabled = false;
		textBoxHighScore.enabled = false;

		highScores.CheckScore(newScore);

		textBoxHighScorePanel.text = "High Score:\n" + highScores.ToString();
		textBoxScorePanel.text = "Player Score:\n" + newScore.ToString();
	}

	public class Score
	{
		Player player;
		public float distance { get; protected set; }
		public int jumps { get; protected set; }
		public int colorChagnes { get; protected set; }
		bool isFinal;

		public ScoreManager manager;

		public Score(Player person, ScoreManager man) {
			manager = man;
			player = person;
			if (player != null) player.score = this;
			isFinal = false;

			distance = 0;
			jumps = 0;
			colorChagnes = 0;
		}

		public Score(Score copy) {
			manager = copy.manager;
			player = copy.player;
			if (player != null) player.score = this;
			isFinal = false;

			distance = copy.distance;
			jumps = copy.jumps;
			colorChagnes = copy.colorChagnes;
		}

		public void change() {
			if (!isFinal) colorChagnes++;
		}
		public void jump() {
			if (!isFinal) jumps++;
		}

		public void finalize() {
			isFinal = true;
			if (manager != null) manager.FinalizeScore(this);
		}

		public override string ToString() {
			return jumps + "\tJumps\n" + colorChagnes + "\tColorChanges";
		}
	}

	public class HighScore : Score
	{
		public HighScore() : base(null, null) { }

		public void CheckScore(Score score) {
			if (score.jumps > jumps) jumps = score.jumps;
			if (score.colorChagnes > colorChagnes) colorChagnes = score.colorChagnes;
			if (score.distance > distance) distance = score.distance;
		}

	}


}
