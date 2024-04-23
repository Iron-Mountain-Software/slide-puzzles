# Slide Puzzles
*Version: 1.0.0*
## Description: 

---
## Key Scripts & Components: 
1. public class **SlidePuzzle** : MonoBehaviour
   * Actions: 
      * public event Action ***OnSolvedChanged*** 
   * Properties: 
      * public Int32 ***Size***  { get; }
      * public Vector2Int ***EmptySpace***  { get; set; }
      * public Boolean ***Solved***  { get; }
   * Methods: 
      * public void ***Initialize***(Int32 size, Texture2D texture)
      * public void ***SetPiece***(Vector2Int coordinates, SlidePuzzlePiece piece)
      * public SlidePuzzlePiece ***GetPiece***(Vector2Int coordinates)
      * public void ***Shuffle***()
      * public Boolean ***IsValidPieceToMove***(Vector2Int coordinates)
      * public Boolean ***IsValidLineToSlide***(Vector2Int coordinates)
1. public class **SlidePuzzlePiece** : MonoBehaviour
   * Actions: 
      * public event Action ***OnCurrentCoordinatesChanged*** 
   * Properties: 
      * public Int32 ***TrueValue***  { get; }
      * public Int32 ***CurrentValue***  { get; }
      * public Vector2Int ***TrueCoordinates***  { get; }
      * public Vector2Int ***CurrentCoordinates***  { get; }
   * Methods: 
      * public SlidePuzzlePiece ***Initialize***(SlidePuzzle slidePuzzle, Vector2Int trueCoordinates)
      * public void ***SetCurrentCoordinates***(Vector2Int currentCoordinates)
      * public void ***SetSprite***(Sprite sprite)
      * public virtual void ***OnPointerDown***(PointerEventData eventData)
