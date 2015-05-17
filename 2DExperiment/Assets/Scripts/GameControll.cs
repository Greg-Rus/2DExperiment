using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameControll : MonoBehaviour {
	
	public static GameControll instance;
	
	public int boardWidth = 6;
	public int boardHeight = 6;
	
	public BoardController boardController;
	public TileController tileController;
	public PipeLayerController pipeLayerController;
	public BoardGenerator boardGenerator;
	public Camera mainCamera;
	public GameObject optionsMenu;
	private Animator menuAnimator;
	public InputField selectedWidth;
	public InputField selectedHeight;
	public Toggle usePipesToggle;
	public PrefabManager prefabManager;
	public GameObject backGroundObject;
	private float score;
	public Text scoreUI;
	// Use this for initialization
	void Awake(){
		instance = this;
		menuAnimator = optionsMenu.GetComponent<Animator>();
	}
	
	void Start()
	{
		//boardController.enabled = false;
		#if UNITY_EDITOR
		//startGame();
		#endif
	}

	
	public void startGame()
	{
		score = 0;
		scoreUI.text = score.ToString();
		setBoardWidthAndHeight();
		initialComponentSetup();
		boardGenerator.setUpBoard();
		if(usePipesToggle.isOn)	pipeLayerController.setUpPipeSegments();
		else pipeLayerController.cleanUpOldPipes();
		
		tileController.StartNewGame();
		//tileController.enabled = true;
		//boardController.startNewGame();
		//boardController.enabled = true;
		hideOptionsMenu();
	}
	
	public void setBoardWidthAndHeight()
	{		
		int.TryParse(selectedWidth.text, out boardWidth);
		int.TryParse(selectedHeight.text, out boardHeight);
	}
	
	public void quitGame()
	{
		Application.Quit();
	}
	
	
	private void initialComponentSetup()
	{
		//boardController.BoardHeight = boardHeight;
		//boardController.BoardWidth = boardWidth;
		
		tileController.BoardHeight = boardHeight;
		tileController.BoardWidth = boardWidth;
		
		pipeLayerController.BoardHeight = boardHeight;
		pipeLayerController.BoardWidth = boardWidth;
		
		boardGenerator.BoardHeight = boardHeight;
		boardGenerator.BoardWidth = boardWidth;
		
		mainCamera.orthographicSize = boardWidth *0.89f; //TODO need to investigate a cleaner solution. For now 0.89 is the percieved size of one tile
		Vector3 boardCenterPos = new Vector3((boardWidth-1) * 0.5f,
		                                     (boardHeight-1) * 0.5f,
		                                     0f);
		mainCamera.transform.position = boardCenterPos - (Vector3.forward * 10);
		backGroundObject.transform.position = boardCenterPos + (Vector3.forward * 10);
	}
	
	public void showOptionsMenu()
	{
		Time.timeScale = 0;
		boardController.enabled = false;
		menuAnimator.Play("OptionsMenuSlideIn");
	}
	public void hideOptionsMenu()
	{
		//boardController.enabled = false; turned on on game start
		Time.timeScale = 1;
		menuAnimator.Play("OptionsMenuSlideOut");
	}
	public void updateScore(int points)
	{
		score += points;
		scoreUI.text = score.ToString();
	}
}
