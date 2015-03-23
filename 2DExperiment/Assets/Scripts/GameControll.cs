using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameControll : MonoBehaviour {
	
	public static GameControll instance;
	
	public int boardWidth = 6;
	public int boardHeight = 6;
	
	public BoardController boardController;
	public PipeLayerController pipeLayerController;
	public BoardGenerator boardGenerator;
	public Camera mainCamera;
	public GameObject optionsMenu;
	private Animator menuAnimator;
	public InputField selectedWidth;
	public InputField selectedHeight;
	// Use this for initialization
	void Awake(){
		instance = this;
		menuAnimator = optionsMenu.GetComponent<Animator>();
	}
	
	void Start()
	{
		boardController.enabled = false;
	}
	
	public void startGame()
	{
		setBoardWidthAndHeight();
		initialComponentSetup();
		boardGenerator.setUpBoard();
		boardController.startNewGame();
		boardController.enabled = true;
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
		boardController.BoardHeight = boardHeight;
		boardController.BoardWidth = boardWidth;
		
		pipeLayerController.BoardHeight = boardHeight;
		pipeLayerController.BoardWidth = boardWidth;
		
		boardGenerator.BoardHeight = boardHeight;
		boardGenerator.BoardWidth = boardWidth;
		
		mainCamera.orthographicSize = boardWidth *0.89f;
		mainCamera.transform.position = new Vector3((boardWidth-1) * 0.5f,
		                                             (boardHeight-1) * 0.5f,
		                                             -10f);
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
}
