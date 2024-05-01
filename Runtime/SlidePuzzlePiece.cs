using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IronMountain.SlidePuzzles
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(RectTransform))]
    public class SlidePuzzlePiece : MonoBehaviour, IPointerDownHandler
    {
        public event Action OnCauseMovement;
        public event Action OnCurrentCoordinatesChanged;
        
        [SerializeField] private Image image;
        [SerializeField] private float animationSeconds = .2f;

        private RectTransform _rectTransform;
        private SlidePuzzle _slidePuzzle;
        private Vector2Int _trueCoordinates;
        private Vector2Int _currentCoordinates;
        
        public int TrueValue => _trueCoordinates.y * _slidePuzzle.Size + _trueCoordinates.x;
        public int CurrentValue => _currentCoordinates.y * _slidePuzzle.Size + _currentCoordinates.x;

        public Vector2Int TrueCoordinates => _trueCoordinates;
        public Vector2Int CurrentCoordinates
        {
            get => _currentCoordinates;
            private set
            {
                if (_currentCoordinates == value) return;
                _currentCoordinates = value;
                OnCurrentCoordinatesChanged?.Invoke();
            }
        }
        
        private void OnValidate()
        {
            if (!_rectTransform) _rectTransform = GetComponent<RectTransform>();
            if (!image) image = GetComponentInChildren<Image>();
            if (animationSeconds < 0) animationSeconds = 0;
        }
        
        private void Awake() => OnValidate();

        public SlidePuzzlePiece Initialize(SlidePuzzle slidePuzzle, Vector2Int trueCoordinates)
        {
            OnValidate();
            _slidePuzzle = slidePuzzle;
            _trueCoordinates = trueCoordinates;
            _currentCoordinates = trueCoordinates;
            SnapPosition();
            return this;
        }

        public void SetCurrentCoordinates(Vector2Int currentCoordinates)
        {
            CurrentCoordinates = currentCoordinates;
            SnapPosition();
        }

        private void SnapPosition()
        {
            if (!_slidePuzzle) return;
            _rectTransform.anchorMin = new Vector2((float) CurrentCoordinates.x / _slidePuzzle.Size, (float) (_slidePuzzle.Size - 1 - CurrentCoordinates.y) / _slidePuzzle.Size);
            _rectTransform.anchorMax = new Vector2((float) (CurrentCoordinates.x + 1) / _slidePuzzle.Size, (float) (_slidePuzzle.Size - 1 - CurrentCoordinates.y + 1) / _slidePuzzle.Size);
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;
        }

        private IEnumerator LerpPosition(Vector2 startAnchorMin, Vector2 startAnchorMax, Vector2 endAnchorMin, Vector2 endAnchorMax)
        {
            for (float timer = 0f; timer < animationSeconds; timer += Time.deltaTime)
            {
                float progress = timer / animationSeconds;
                _rectTransform.anchorMin = Vector2.Lerp(startAnchorMin, endAnchorMin, progress);
                _rectTransform.anchorMax = Vector2.Lerp(startAnchorMax, endAnchorMax, progress);
                _rectTransform.offsetMin = Vector2.zero;
                _rectTransform.offsetMax = Vector2.zero;
                yield return null;
            }
            _rectTransform.anchorMin = endAnchorMin;
            _rectTransform.anchorMax = endAnchorMax;
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;
        }

        public void SetSprite(Sprite sprite)
        {
            if (!image) return;
            image.sprite = sprite;
            image.preserveAspect = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_slidePuzzle) return;
            if (_slidePuzzle.IsValidPieceToMove(CurrentCoordinates))
            {
                Move();
                OnCauseMovement?.Invoke();
            }
            else if (_slidePuzzle.IsValidLineToSlide(CurrentCoordinates))
            {
                List<Vector2Int> coordinatesToSlide = new List<Vector2Int>();
                if (CurrentCoordinates.y == _slidePuzzle.EmptySpace.y)
                {
                    if (CurrentCoordinates.x > _slidePuzzle.EmptySpace.x)
                    {
                        for (int x = _slidePuzzle.EmptySpace.x + 1; x <= CurrentCoordinates.x; x++)
                            coordinatesToSlide.Add(new Vector2Int(x, CurrentCoordinates.y));
                    }
                    else if (CurrentCoordinates.x < _slidePuzzle.EmptySpace.x)
                    {
                        for (int x = _slidePuzzle.EmptySpace.x - 1; x >= CurrentCoordinates.x; x--)
                            coordinatesToSlide.Add(new Vector2Int(x, CurrentCoordinates.y));
                    }
                }
                else if (CurrentCoordinates.x == _slidePuzzle.EmptySpace.x)
                {
                    if (CurrentCoordinates.y > _slidePuzzle.EmptySpace.y)
                    {
                        for (int y = _slidePuzzle.EmptySpace.y + 1; y <= CurrentCoordinates.y; y++) 
                            coordinatesToSlide.Add(new Vector2Int(CurrentCoordinates.x, y));
                    }
                    else if (CurrentCoordinates.y < _slidePuzzle.EmptySpace.y)
                    {
                        for (int y = _slidePuzzle.EmptySpace.y - 1; y >= CurrentCoordinates.y; y--) 
                            coordinatesToSlide.Add(new Vector2Int(CurrentCoordinates.x, y));
                    }
                }
                foreach (Vector2Int coordinates in coordinatesToSlide)
                {
                    SlidePuzzlePiece piece = _slidePuzzle.GetPiece(coordinates);
                    if (piece) piece.Move();
                }
                OnCauseMovement?.Invoke();
            }
        }

        private void Move()
        {
            if (!_slidePuzzle || !_slidePuzzle.IsValidPieceToMove(CurrentCoordinates)) return;
            Vector2Int previousCoordinates = CurrentCoordinates;
            CurrentCoordinates = _slidePuzzle.EmptySpace;
            _slidePuzzle.EmptySpace = previousCoordinates;
            _slidePuzzle.SetPiece(previousCoordinates, null);
            _slidePuzzle.SetPiece(CurrentCoordinates, this);
            StopAllCoroutines();
            StartCoroutine(LerpPosition(
                _rectTransform.anchorMin,
                _rectTransform.anchorMax,
                new Vector2((float) CurrentCoordinates.x / _slidePuzzle.Size, (float) (_slidePuzzle.Size - 1 - CurrentCoordinates.y) / _slidePuzzle.Size),
                new Vector2((float) (CurrentCoordinates.x + 1) / _slidePuzzle.Size, (float) (_slidePuzzle.Size - 1 - CurrentCoordinates.y + 1) / _slidePuzzle.Size)
            ));
        }
    }
}
