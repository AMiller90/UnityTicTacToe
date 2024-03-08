using Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    /// <summary>
    /// Reference to the slot ui components in the game board
    /// </summary>
    private SlotUIComponent[,] GameBoard;

    /// <summary>
    /// Reference to a list of the empty slot ui components in the game board
    /// </summary>
    public List<SlotUIComponent> EmptySlots { get; private set; }

    [Header("References")]
    /// <summary>
    /// Reference to the slot prefab
    /// </summary>
    [SerializeField] private GameObject _slotPrefab;

    [Header("Hud Components")]
    /// <summary>
    /// Reference to the grid layout group for the gameboard
    /// </summary>
    [SerializeField] private GridLayoutGroup _gameboardLayoutGroup;

    /// <summary>
    /// Reference to the tic tac toe board panel
    /// </summary>
    [SerializeField] private GameObject _ticTacToeBoardPanel;

    /// <summary>
    /// Reference to the board block panel
    /// </summary>
    [SerializeField] private GameObject _boardBlockPanel;

    /// <summary>
    /// Reference to the game start container
    /// </summary>
    [SerializeField] private GameObject _gameStartContainer;
    
    /// <summary>
    /// Reference to the game start grid size drop down
    /// </summary>
    [SerializeField] private TMPro.TMP_Dropdown _gameStartGridSizeDropdown;
    
    /// <summary>
    /// Reference to the game start character size drop down
    /// </summary>
    [SerializeField] private TMPro.TMP_Dropdown _gameStartCharacterDropdown;
    
    /// <summary>
    /// Reference to the game over container
    /// </summary>
    [SerializeField] private GameObject _gameOverContainer;
    
    /// <summary>
    /// Reference to the game over grid size drop down
    /// </summary>
    [SerializeField] private TMPro.TMP_Dropdown _gameOverGridSizeDropdown;
    
    /// <summary>
    /// Reference to the game over text
    /// </summary>
    [SerializeField] private TMPro.TMP_Text _gameOverText;
    
    /// <summary>
    /// Reference to the game over character size drop down
    /// </summary>
    [SerializeField] private TMPro.TMP_Dropdown _gameOverCharacterDropdown;

    [Header("Audio")]
    /// <summary>
    /// Reference to the audio source
    /// </summary>
    [SerializeField] private AudioSource _audioSource;

    /// <summary>
    /// Reference to the game controller for singleton pattern
    /// </summary>
    private static GameController _gameController;
    
    /// <summary>
    /// Reference to the game controller for singleton pattern
    /// </summary>
    public static GameController Instance => _gameController;

    /// <summary>
    /// Reference to the players
    /// </summary>
    private Player[] _thePlayers;

    /// <summary>
    /// Reference to the current player
    /// </summary>
    private Player _currentPlayer;
    
    /// <summary>
    /// Reference to the number of turns that have taken place
    /// </summary>
    private int _numberOfTurnsTaken;
    
    /// <summary>
    /// Reference to the size of the board chosen by the user
    /// </summary>
    private int _boardSize;

    void Awake()
    {
        _gameController = this;
    }

    /// <summary>
    /// Function used to initialize and set up the board.
    /// Uses IEnumerator to delay instantiation of each slot on the board giving an animation affect.
    /// </summary>
    /// <param name="maxSize">the max size of the board chosen by the user</param>
    /// /// <returns>Uses IEnumerator to delay instantiation of each slot on the board</returns>
    private IEnumerator InitializeBoard(int maxSize)
    {
        _boardSize = maxSize;
        _gameboardLayoutGroup.constraintCount = maxSize;
        EmptySlots = new List<SlotUIComponent>();
        GameBoard = new SlotUIComponent[maxSize, maxSize];
        int delay = 2 / (maxSize * maxSize);

        _boardBlockPanel.SetActive(true);
        for (int i = 0; i < maxSize; i++)
        {
            for (int j = 0; j < maxSize; j++)
            {
                GameObject slot = Instantiate(_slotPrefab, _ticTacToeBoardPanel.transform);
                SlotUIComponent slotUIComponent = slot.GetComponent<SlotUIComponent>();
                slotUIComponent.Initialize(new Slot(i, j), null);
                GameBoard[i, j] = slotUIComponent;
                EmptySlots.Add(slotUIComponent);
                yield return new WaitForSeconds(delay);
            }
        }

        _boardBlockPanel.SetActive(false);
    }

    /// <summary>
    /// Function used to clear the board
    /// </summary>
    private void ClearBoard()
    {
        foreach (Transform child in _ticTacToeBoardPanel.transform) {
            Destroy(child.gameObject);
        }
        EmptySlots.Clear();
    }
    
    /// <summary>
    /// Function used to change player turn
    /// </summary>
    private void ChangePlayerTurn()
    {
        this._thePlayers[0].myTurn = !this._thePlayers[0].myTurn;
        this._thePlayers[1].myTurn = !this._thePlayers[1].myTurn;
        
        _currentPlayer = this._thePlayers[0].myTurn ? this._thePlayers[0] : this._thePlayers[1];
        
        if(_currentPlayer is AIPlayer)
            ProcessMove(null);
    }

    /// <summary>
    /// Function to check if the game has a winner
    /// </summary>
    /// <returns>Returns true or false whether a player has won</returns>
    private bool GameHasWinner()
    {
        int xCoord = this._currentPlayer.coords[0];
        int yCoord = this._currentPlayer.coords[1];

        // Check Column
        for (int i = 0; i < this._boardSize; i++)
        {
            if (this.GameBoard[i, yCoord].Slot.character != this._currentPlayer.playerChar)
                break;
            
            if (i == _boardSize - 1)
                return true;
        }
        
        // Check Row
        for (int i = 0; i < this._boardSize; i++)
        {
            if (this.GameBoard[xCoord, i].Slot.character != this._currentPlayer.playerChar)
                break;
            if (i == _boardSize - 1)
                return true;
        }
        
        // Check Diagonal From Top Left To Bottom Right
        for (int i = 0; i < this._boardSize; i++)
        {
            if (this.GameBoard[i, i].Slot.character != this._currentPlayer.playerChar)
                break;
            if (i == _boardSize - 1)
                return true;
        }

        // Check Diagonal From Top Right to Bottom Left
        for (int i = 0; i < this._boardSize; i++)
        {
            if (this.GameBoard[i, (this._boardSize - 1) - i].Slot.character != this._currentPlayer.playerChar)
                break;
            if (i == _boardSize - 1)
                return true;
        }
        
        return false;
    }

    /// <summary>
    /// Function used to check if the game is a tie
    /// </summary>
    /// <returns>Returns true or false whether their is a tie</returns>
    private bool IsTieGame()
    {
        return _numberOfTurnsTaken == (this._boardSize * this._boardSize);
    }
    
    /// <summary>
    /// Function used to process the most recent move that has taken place
    /// </summary>
    /// <param name="slot">the slot of the most recent move</param>
    public void ProcessMove(Slot slot)
    {
        int[] coordsSlotChosen = _currentPlayer.TakeTurn(slot);
        SlotUIComponent slotUi = this.GameBoard[coordsSlotChosen[0], coordsSlotChosen[1]];
        slotUi.UpdateSlot(_currentPlayer.playerChar);
        EmptySlots.Remove(slotUi);

        _numberOfTurnsTaken++;
        _audioSource.Play();
        // If Number of turns is less than turns for there to be a possible winner
        if (_numberOfTurnsTaken < (this._boardSize * 2) - 1)
        {
            this.ChangePlayerTurn();
            return;
        }

        if (GameHasWinner())
        {
            _gameOverText.text = "Player " + _currentPlayer.playerChar + " wins!";
            _gameOverContainer.SetActive(true);
            _boardBlockPanel.SetActive(true);
            return;
        }
        
        if (IsTieGame())
        {
            _gameOverText.text = "Tie Game!";
            _gameOverContainer.SetActive(true);
            _boardBlockPanel.SetActive(true);
            return;
        }
        
        this.ChangePlayerTurn();
    }

    /// <summary>
    /// Function to check if a slot is empty based on passed in coords
    /// </summary>
    /// <param name="coords">the coords to check in the board</param>
    /// <returns>Returns true or false if the slot at the coords passed in is empty</returns>
    public bool SlotIsEmpty(int[] coords)
    {
        return this.GameBoard[coords[0], coords[1]].Slot.character == ' ';
    }

    /// <summary>
    /// Function used to start a new game.
    /// This is called in the inspector on Play Game button click event
    /// </summary>
    public void NewGame()
    {
        this.StartCoroutine(NewGameCallback());
    }

    /// <summary>
    /// Function used to play the game again.
    /// This is called in the inspector on Play Again button click event
    /// </summary>
    public void PlayAgain()
    {
        this.StartCoroutine(PlayAgainCallback());
    }

    /// <summary>
    /// Function used for new game logic
    /// </summary>
    /// <returns></returns>
    private IEnumerator NewGameCallback()
    {
        _audioSource.Play();
        _thePlayers = new Player[2];
        if (_gameStartCharacterDropdown.value == 0)
        {
            _thePlayers[0] = new RealPlayer('X');
            _thePlayers[0].myTurn = true;
            _currentPlayer = _thePlayers[0];
            _thePlayers[1] = new AIPlayer('O');
        }
        else
        {
            _thePlayers[0] = new RealPlayer('O');
            _thePlayers[1] = new AIPlayer('X');
            _thePlayers[1].myTurn = true;
            _currentPlayer = _thePlayers[1];
        }

        _ticTacToeBoardPanel.SetActive(true);
        _gameStartContainer.SetActive(false);
        yield return this.StartCoroutine(this.InitializeBoard(_gameStartGridSizeDropdown.value + 3));

        if (_currentPlayer is AIPlayer)
            this.ProcessMove(null);
    }

    /// <summary>
    /// Function used for play again logic
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayAgainCallback()
    {
        _audioSource.Play();
        _numberOfTurnsTaken = 0;
        this.ClearBoard();
        _thePlayers = new Player[2];
        if (_gameOverCharacterDropdown.value == 0)
        {
            _thePlayers[0] = new RealPlayer('X');
            _thePlayers[0].myTurn = true;
            _currentPlayer = _thePlayers[0];
            _thePlayers[1] = new AIPlayer('O');
        }
        else
        {
            _thePlayers[0] = new RealPlayer('O');
            _thePlayers[1] = new AIPlayer('X');
            _thePlayers[1].myTurn = true;
            _currentPlayer = _thePlayers[1];
        }
        _gameOverContainer.SetActive(false);
        yield return this.StartCoroutine(this.InitializeBoard(_gameOverGridSizeDropdown.value + 3));

        if (_currentPlayer is AIPlayer)
            this.ProcessMove(null);
    }

    /// <summary>
    /// Function used to quit the game.
    /// This is called on the Quit button click events.
    /// </summary>
    public void QuitGame()
    {
        _audioSource.Play();
        Application.Quit();
    }
}
