using System;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonTest
{
    class Block
    {
        public const byte CEMENT = 0;
        public const byte CEMENT_BRICK = 1;

        public static TextureData Cement;
        public static TextureData CementBrick;

        //texture for the block to use when drawn
        public TextureData texture;

        // raycast engine only draws solid blocks
        public bool isSolid;
        public byte blockType = 0;

        public Block(byte block, bool solid)
        {
            isSolid = solid;

            blockType = block;
            texture = Cement;
            
            switch (block)
            {
                case CEMENT_BRICK:
                    texture = CementBrick;
                    break;
            }
        }

        // what happens when we click the block
        public virtual void Interact()
        {

        }

        // what happens when we step on this block
        // for floor blocks
        public virtual void Step()
        {

        }
    }

    class Door : Block
    {
        public Door(byte block)
            : base(block, true)
        {
        }

        public override void Interact()
        {
            isSolid = !isSolid;
        }
    }
}
