namespace Dungeontest
{
    public class Block
    {
        public bool solid;

        // block id for the sprite
        public byte blockType = 0;

        public Block(byte block, bool isSolid)
        {
            solid = isSolid;

            blockType = block;
        }
    }
}
