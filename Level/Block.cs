using System;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonTest
{
    class Block
    {
        public const byte COBBLE = 0;
        public const byte WOOD = 1;

        public static TextureData Cobble;
        public static TextureData Wood;

        //texture for the block to use when drawn
        public TextureData texture;

        // raycast engine only draws solid blocks
        public bool isSolid;
        public byte blockType = 0;

        public Block(byte block, bool solid)
        {
            isSolid = solid;

            blockType = block;
            texture = Cobble;
            
            switch (block)
            {
                case COBBLE:
                    break;
                case WOOD:
                    texture = Wood;
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
