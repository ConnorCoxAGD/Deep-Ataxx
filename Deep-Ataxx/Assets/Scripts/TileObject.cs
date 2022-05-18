using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cox.Infection.Management{
    public class TileObject : MonoBehaviour
    {
        PlayerHelper player;

        public bool isDisabled = false;
        public PieceComponent piece = null;
        public Vector2Int gridPosition;
        public List<TileObject> neighbors;
        public GameObject blockade;

        private void Awake() {
            player = FindObjectOfType<PlayerHelper>();
            blockade.SetActive(false);
        }

        private void OnMouseEnter() {
            player.hoveredTile = this;
        }

        public void FindNeighbors(){
            TileObject[] tiles = FindObjectsOfType<TileObject>();
            foreach(var tile in tiles){
                if(tile == this)continue;
                if(Vector2Int.Distance(gridPosition, tile.gridPosition) < 2){
                    neighbors.Add(tile);
                }
            }
        }

        public void BlockTile(bool isBlocked){
            isDisabled = isBlocked;
            if(isDisabled){
                blockade.SetActive(true);
            }

        }
    }
}

