using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour {

    [Header("Board Variables")]
    public int column;
    public int row;
    public int targetX;
    public int targetY;

    private FindMatches findMatches;
    private Board board;
    private GameObject neighbourDot;

    private Vector2 tempPosition;

    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    public bool isMatched = false;
    public int previousColumn;
    public int previousRow;

    // Start is called before the first frame update
    void Start() {
        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
        //targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;
        //row = targetY;
        //column = targetX;
        //previousRow = row;
        //previousColumn = column;
    }

    // Update is called once per frame
    void Update() {
        //FindMatches();
        if(isMatched) {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(1f, 1f, 1f, .2f);
        }

        targetX = column;
        targetY = row;
        if(Mathf.Abs(targetX - transform.position.x) > .1) {
            //move to the target
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
            if(board.allDots[column, row] != this.gameObject) {
                board.allDots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else {
            //directly set position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1) {
            //move to the target
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
            if (board.allDots[column, row] != this.gameObject) {
                board.allDots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else {
            //directly set position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }
    }

    public IEnumerator CheckMove() {
        yield return new WaitForSeconds(.5f);
        if(neighbourDot != null) { 
            if(!isMatched && !neighbourDot.GetComponent<Dot>().isMatched) {
                neighbourDot.GetComponent<Dot>().row = row;
                neighbourDot.GetComponent<Dot>().column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(.5f);
                board.currentState = GameState.move;
            } else {
                board.DestroyMatches();
            }
            neighbourDot = null;
        } 
    }

    private void OnMouseDown() {
        if (board.currentState == GameState.move) {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp() {
        if (board.currentState == GameState.move) {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    private void CalculateAngle() {
        if(Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist) {
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            Debug.Log(swipeAngle);
            MovePieces();
            board.currentState = GameState.wait;
        } else {
            board.currentState = GameState.move;
        }
    }

    void MovePieces() {
        switch(swipeAngle) {
            case var expression when (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1):
                //swipe right
                neighbourDot = board.allDots[column + 1, row];
                previousRow = row;
                previousColumn = column;
                neighbourDot.GetComponent<Dot>().column--;
                column++;
                break;
            case var expression when (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1):
                //swipe up
                neighbourDot = board.allDots[column, row + 1];
                previousRow = row;
                previousColumn = column;
                neighbourDot.GetComponent<Dot>().row--;
                row++;
                break;
            case var expression when ((swipeAngle > 135 || swipeAngle <= -135) && column > 0 && row > 0):
                //swipe left
                neighbourDot = board.allDots[column - 1, row];
                previousRow = row;
                previousColumn = column;
                neighbourDot.GetComponent<Dot>().column++;
                column--;
                break;
            case var expression when (swipeAngle < -45 && swipeAngle >= -135 && row > 0):
                //swipe down
                neighbourDot = board.allDots[column, row - 1];
                previousRow = row;
                previousColumn = column;
                neighbourDot.GetComponent<Dot>().row++;
                row--;
                break;
            default:
                break;
        }
        StartCoroutine("CheckMove");
    }

    void FindMatches() {
        if(column > 0 && column < board.width - 1) {
            GameObject leftDot1 = board.allDots[column - 1, row];
            GameObject rightDot1 = board.allDots[column + 1, row];
            if (leftDot1 != null && rightDot1 != null) {
                if (leftDot1.tag == this.gameObject.tag && this.gameObject.tag == rightDot1.tag) {
                    leftDot1.GetComponent<Dot>().isMatched = true;
                    rightDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
        if (row > 0 && row < board.height - 1) {
            GameObject upDot1 = board.allDots[column, row + 1];
            GameObject downDot1 = board.allDots[column, row - 1];
            if (upDot1 != null && downDot1 != null) {
                if (upDot1.tag == this.gameObject.tag && this.gameObject.tag == downDot1.tag) {
                    upDot1.GetComponent<Dot>().isMatched = true;
                    downDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }
}
