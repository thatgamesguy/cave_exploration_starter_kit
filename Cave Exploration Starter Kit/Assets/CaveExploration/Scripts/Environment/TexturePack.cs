using UnityEngine;
using System.Collections;

namespace CaveExploration
{
	/// <summary>
	/// Singleton. Provides a centralised location for block tetures. 
	/// Provides a method to retrieve a texture based on a node type.
	/// </summary>
	public class TexturePack : MonoBehaviour
	{
		/// <summary>
		/// Whether this texture pack can be used in the game.
		/// </summary>
		public bool Enabled = true;

		/// <summary>
		/// The wall top left sprite.
		/// </summary>
		public Sprite WallTopLeft;

		/// <summary>
		/// The wall top middle sprite.
		/// </summary>
		public Sprite WallTopMiddle;

		/// <summary>
		/// The wall top right sprite.
		/// </summary>
		public Sprite WallTopRight;

		/// <summary>
		/// The wall middle left sprite.
		/// </summary>
		public Sprite WallMiddleLeft;

		/// <summary>
		/// The wall middle sprite.
		/// </summary>
		public Sprite WallMiddle;

		/// <summary>
		/// The wall middle right sprite.
		/// </summary>
		public Sprite WallMiddleRight;

		/// <summary>
		/// The wall bottom left sprite.
		/// </summary>
		public Sprite WallBottomLeft;

		/// <summary>
		/// The wall bottom middle sprite.
		/// </summary>
		public Sprite WallBottomMiddle;

		/// <summary>
		/// The wall bottom right sprite.
		/// </summary>
		public Sprite WallBottomRight;

		/// <summary>
		/// The background sprite.
		/// </summary>
		public Sprite Background;

		/// <summary>
		/// Returns a texture based on a node type.
		/// </summary>
		public Sprite GetSpriteFromCellType (NodeType cellType)
		{

			Sprite sprite = null;

			switch (cellType) {
			case NodeType.WallTopLeft:
				sprite = WallTopLeft;
				break;
			case NodeType.WallTopMiddle:
				sprite = WallTopMiddle;
				break;
			case NodeType.WallTopRight:
				sprite = WallTopRight;
				break;
			case NodeType.WallMiddleLeft:
				sprite = WallMiddleLeft;
				break;
			case NodeType.WallMiddle:
				sprite = WallMiddle;
				break;
			case NodeType.WallMiddleRight:
				sprite = WallMiddleRight;
				break;
			case NodeType.WallBottomLeft:
				sprite = WallBottomLeft;
				break;
			case NodeType.WallBottomMiddle:
				sprite = WallBottomMiddle;
				break;
			case NodeType.WallBottomRight:
				sprite = WallBottomRight; 
				break; 
			case NodeType.Background:
				sprite = Background;
				break;
			case NodeType.Entry:
				sprite = WallMiddle;
				break;
			default:
				sprite = WallMiddle;
				break;
			}

			if (!sprite) {
				Debug.LogError (cellType + " not set");
			}

			return sprite;

		}

		/// <summary>
		/// Gets the size of the sprite.
		/// </summary>
		/// <returns>The sprite size.</returns>
		/// <param name="cellType">Cell type.</param>
		/// <param name="localScale">Local scale.</param>
		public Vector2 GetSpriteSize ()
		{
			Sprite sprite = GetSpriteFromCellType (NodeType.Background);

			var localScale = Utilities.instance.nodeScale;

			return new Vector2 (sprite.bounds.size.x * localScale.x, sprite.bounds.size.y * localScale.y);
		}

	



	
	
	}
}
