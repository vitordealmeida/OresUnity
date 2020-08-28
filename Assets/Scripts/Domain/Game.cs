namespace Domain.Entities
{
    public class Game
    {
        private const int InitialOreTypes = 4;

        public Board Board { get; }

        public Game()
        {
            Board = new Board(4);
        }

        public override string ToString()
        {
            return Board.ToString();
        }
    }
}