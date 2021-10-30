using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {
  private Text _magazineText;

  void Awake() {
    _magazineText = GameObject.Find("Magazine_Text").GetComponent<Text>();
  }

  public void UpdateMagazine(int magazine, int magazineSize, bool reloading = false) {
    if (reloading) {
      _magazineText.text = "Reloading...";
    } else {
      _magazineText.text = magazine + "/" + magazineSize;
    }
  }
}