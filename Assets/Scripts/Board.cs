﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

    public int width;
    public int height;
    public GameObject tilePrefab;
    private BackgroundTile[,] allTiles;

    public GameObject[] dots;

    public GameObject[,] allDots;

    // Start is called before the first frame update
    void Start() {
        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
        Initialization();
    }

    private void Initialization() {
        for(int i = 0; i < width; i++) {
            for(int j = 0; j < height; j++) {
                Vector2 tempPosition = new Vector2(i, j);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "(" + i + ", " + j + " )";

                int randomDot = Random.Range(0, dots.Length);
                GameObject dot = Instantiate(dots[randomDot], tempPosition, Quaternion.identity);
                dot.transform.parent = this.transform;
                dot.name = "(" + i + ", " + j + " )";
                allDots[i, j] = dot;
            }
        }
    }
}