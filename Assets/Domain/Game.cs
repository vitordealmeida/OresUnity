namespace Domain.Entities
{
    public class Game
    {
        private const int InitialOreTypes = 4;
        
        private Board _board;

        public Game()
        {
            _board = new Board(4);
        }

        public override string ToString()
        {
            return _board.ToString();
        }
    }
}