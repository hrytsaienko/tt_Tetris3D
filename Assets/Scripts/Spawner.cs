using UnityEngine;

public class Spawner : MonoBehaviour {

    [SerializeField]
    private GameObject[] groups;

    private GameObject previewTetromino;
    private GameObject nextTetromino;

    private Vector3 previewTetrominoPosition = new Vector3(11.5f, 15.0f, 0.0f);
    private Vector3 nextTetrominoPosition = new Vector3(5.0f, 17.0f, 0.0f);

    public void SpawnNext()
    {
        int index = Random.Range(0, groups.Length);

        if (!Board.gameStarted)
        {
            Board.gameStarted = true;

            // Spawn very fist tetromino in the gaame 
            nextTetromino = Instantiate(groups[index], nextTetrominoPosition,
                        Quaternion.identity);

            // new index for preview, tetromino, which will be next after current one 
            index = Random.Range(0, groups.Length);           
        }
        else
        {
            //if game is starded and we already have a preview tetromino, set it up
            previewTetromino.transform.localPosition = nextTetrominoPosition;
            nextTetromino = previewTetromino;
            nextTetromino.GetComponent<Tetromino>().enabled = true;
        }

        //and spawn new one
        previewTetromino = Instantiate(groups[index], previewTetrominoPosition,
                       Quaternion.identity);
        previewTetromino.GetComponent<Tetromino>().enabled = false;
    }
}
