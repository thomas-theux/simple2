﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Rewired;

public class PlayerInstance : MonoBehaviour {

    public int PlayerID = 0;
    // public int PlayerScore = 0;
    public int SelectedRacerIndex = 0;

    public GameObject PlayerFlag;
    public TMP_Text ScoreText;

    private int navigationIndex = 0;

    private float startPosZ = 0;
    private float moveFlagZAxis = 2.0f;

    private Player player;


    // REWIRED
    private bool confirmBTN = false;
    private bool cancelBTN = false;
    private bool optionBTN = false;

    private bool dpadLeft = false;
    private bool dpadRight = false;
    private bool dpadUp = false;
    private bool dpadDown = false;

    private float leftStickHorizontal = 0;
    private float leftStickVertical = 0;


    private void Start() {
        player = ReInput.players.GetPlayer(PlayerID);

        startPosZ = PlayerFlag.transform.position.z;

        DisplayPlayerScore();
    }


    private void Update() {
        GetInput();
        // ProcessInput();

        if (GameSettings.NavigationMode == 0) {
            if (navigationIndex == 0) {
                RacerSelection();
            }

            if (navigationIndex == 1) {
                CancelSelection();
            }
        }

        if (GameSettings.NavigationMode == 3) {
            if (confirmBTN) {
                if (PlayerID == 0) {
                    NextRace();
                }
            }
        }
    }


    private void GetInput() {
        confirmBTN = player.GetButtonDown("Confirm");
        cancelBTN = player.GetButtonDown("Cancel");
        optionBTN = player.GetButtonDown("Option");
        
        dpadLeft = player.GetButtonDown("D-Pad Left");
        dpadRight = player.GetButtonDown("D-Pad Right");
        dpadUp = player.GetButtonDown("D-Pad Up");
        dpadDown = player.GetButtonDown("D-Pad Down");
        
        leftStickHorizontal = player.GetAxis("Left Stick Horizontal");
        leftStickVertical = player.GetAxis("Left Stick Vertical");
    }


    private void RacerSelection() {
        if (dpadUp) {

            if (SelectedRacerIndex < GameSettings.PlayerMax - 1) {
                AudioManager.instance.PlayRandom("Navigate Vertical", 0.9f, 0.9f);
                SelectedRacerIndex++;
            }
            // else SelectedRacerIndex = 0;
            DisplayPlayerFlag();
        }
        
        if (dpadDown) {

            if (SelectedRacerIndex > 0) {
                SelectedRacerIndex--;
                AudioManager.instance.PlayRandom("Navigate Vertical", 0.8f, 0.8f);
            }
            // else SelectedRacerIndex = GameSettings.PlayerMax - 1;
            DisplayPlayerFlag();
        }

        // Select racer
        if (confirmBTN) {
            if (SelectionManager.SelectedRacers.IndexOf(SelectedRacerIndex) < 0) {
                AudioManager.instance.Play("Confirm");

                navigationIndex = 1;
                SelectionManager.SelectedRacers[PlayerID] = SelectedRacerIndex;

                SpawnRacers.AllRacersArr[SelectedRacerIndex].GetComponent<RacerInstance>().RacerText.color = ColorManager.PlayerColors[PlayerID];

                SelectionManager.DoneSelecting++;
                StartCoroutine(SelectionManager.ContinueDelay());
            } else {
                // Can't select this racer because someone else already selected it
                AudioManager.instance.Play("Cancel");
            }
        }
    }


    private void DisplayPlayerFlag() {
        float newFlagPosZ = startPosZ + (moveFlagZAxis * SelectedRacerIndex);

        PlayerFlag.transform.localPosition = new Vector3(
            PlayerFlag.transform.localPosition.x,
            PlayerFlag.transform.localPosition.y,
            newFlagPosZ
        );
    }


    public void DisplayPlayerScore() {
        ScoreText.text = ResultsManager.PlayerScores[PlayerID] + "";
    }


    private void CancelSelection() {
        // Cancel selection
        if (cancelBTN) {
            AudioManager.instance.Play("Cancel");

            navigationIndex = 0;
            SelectionManager.SelectedRacers[PlayerID] = -1;

            SpawnRacers.AllRacersArr[SelectedRacerIndex].GetComponent<RacerInstance>().RacerText.color = ColorManager.KeyWhite;

            SelectionManager.DoneSelecting--;
        }
    }


    private void NextRace() {
        AudioManager.instance.Play("Next Race");

        SelectionManager.CleanUpSelectionManager();
        SpawnRacers.CleanUpSpawnRacers();
        SpawnPlayers.CleanUpSpawnPlayers();

        GameSettings.NavigationMode = 0;

        SceneManager.LoadScene("2 Meadows");
    }

}
