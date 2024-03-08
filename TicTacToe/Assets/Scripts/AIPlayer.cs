using Random = System.Random;

namespace Scripts
{
    /// <summary>
    /// Child class to represent an ai player
    /// </summary>
    public class AIPlayer : Player
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public AIPlayer()
        {
            this.myTurn = false;
            this.playerChar = ' ';
            this.coords = new int[2];
        }
        
        /// <summary>
        /// AIPlayer overloaded constructor
        /// </summary>
        /// <param name="character">the character to set</param>
        public AIPlayer(char character)
        {
            // Set the turn to false
            this.myTurn = false;
            // Set the character to the passed in character
            this.playerChar = character;
            // Initialize the coordinates
            this.coords = new int[2];
        }

        /// <summary>
        /// Function overloaded to handle ai player functionality
        /// </summary>
        /// <param name="slot">the slot that is being sent</param>
        /// <returns>Returns the coords that have been chosen</returns>
        public override int[] TakeTurn(Slot slot)
        {
            Random rnd = new Random();
            // Get a random index between 0 and the number of empty slots in the empty slots list
            var index = rnd.Next(0, GameController.Instance.EmptySlots.Count);
            // Use the index to get an empty slot from the list
            var slotChosen = GameController.Instance.EmptySlots[index];
            // Set the coords to the chose slots coords
            this.coords = new[] { slotChosen.Slot.xPosition, slotChosen.Slot.yPosition };
            return this.coords;
        }
    }
}