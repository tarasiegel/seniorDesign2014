using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class promptScript {
	protected Sentence firstSentence, secondSentence, thirdSentence, currentSentence;
	string sampleSen = "the_quick_brown_fox";
	string secondSen = "jumped_over_the";
	string thirdSen = "lazy_dog";
	int index = 0;
	List<Sentence> sentenceArray;


	public promptScript(){
		firstSentence = new Sentence (sampleSen);
		secondSentence = new Sentence (secondSen);
		thirdSentence = new Sentence (thirdSen);
		sentenceArray = new List<Sentence>();
		sentenceArray.Add (firstSentence);
		sentenceArray.Add (secondSentence);
		sentenceArray.Add (thirdSentence);
		currentSentence = sentenceArray [index];
	}

	public Sentence sentence{
		get { return currentSentence; }
		set { currentSentence = value; }
	}

	public void proceedToNextLetter() {
		sentence.LettersCompleted.Add (sentence.CurrentLetter);
		sentence.CurrentLetter = sentence.LettersToGo [0];
		sentence.LettersToGo.RemoveAt (0);
		
		if (sentence.LettersToGo.Count == 0) {
			index++;
			currentSentence = sentenceArray[index];
		}
	}

	public class Sentence {
		private List<string> lettersCompleted;
		private string currentLetter;
		private List<string> lettersToGo;

		public List<string> LettersCompleted {
			get { return lettersCompleted; }
			set { lettersCompleted = value;}
		}
		public List<string> LettersToGo {
			get { return lettersToGo; }
			set { lettersToGo = value;}
		}
		public string CurrentLetter {
			get { return currentLetter;}
			set { currentLetter = value;}
		}
		
		public Sentence(string sen) {
			lettersCompleted = new List<string>();
			lettersToGo = new List<string>();
			foreach (char c in sen.ToCharArray()){
				lettersToGo.Add(c.ToString());
			}
			currentLetter = lettersToGo[0];
			lettersToGo.RemoveAt(0);
		}

		public bool checkLetter(string l) {
			if (l == currentLetter || (l == " " && currentLetter == "_")) {
				return true;
			} 
			else {
				return false;
			}
		}
		
		public string getLettersCompleted(){
			string s = "";
			foreach (string l in lettersCompleted) {
				s += l;
			}
			return s;
		}
		
		public string getLettersToGo(){
			string s = "";
			foreach (string l in lettersToGo) {
				s += l;
			}
			return s;
		}
		
		public string getcurrentLetter(){
			return currentLetter;
		}
	}

}
