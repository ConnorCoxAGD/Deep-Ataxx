using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cox.Infection.Utilities;

namespace Cox.Infection.Management{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PieceComponent : MonoBehaviour
    {
        public bool isPlayable;
        public PieceComponent prefab;
        public Team team;
        public TileObject homeTile;
        public List<TileObject> playableTiles = new List<TileObject>();
        TileObject endTile;

        GameManager gm;
        SpriteRenderer sr;
        LineRenderer lr;
        PlayerHelper player;
        Animator animator;

        private void Awake() {
            gm = FindObjectOfType<GameManager>();
            sr = GetComponent<SpriteRenderer>();
            lr = GetComponentInChildren<LineRenderer>();
            lr.SetPosition(0, transform.position);
            lr.enabled = false;
            animator = GetComponent<Animator>();
            player = FindObjectOfType<PlayerHelper>();
        }

        public void ChangeTeam(Team newTeam) {
            team = newTeam;

            if(team == Team.RedTeam){
                sr.color = ColorManager.red; //sets color
                lr.startColor = ColorManager.red;
                lr.endColor = ColorManager.red;
            }
            if(team == Team.GreenTeam){
                sr.color = ColorManager.green; //sets color
                lr.startColor = ColorManager.green;
                lr.endColor = ColorManager.green;
            }
        }

        public bool CheckPlayability(){
            isPlayable = false;
            playableTiles.Clear();
            playableTiles.TrimExcess();
            foreach(var tile in homeTile.reachableTiles){
                if(tile.isDisabled || tile.piece)continue;
                playableTiles.Add(tile); //this list may become helpful for AI, that's why we're adding it in and hints. We could also highlight a piece's potential moves.
            }
            if(playableTiles.Count > 0)isPlayable = true;
            return isPlayable;
        }

        #region  Movement
        void PieceMoved(){
            if(homeTile.gridPosition == endTile.gridPosition){
                ReturnToHome();
            }
            else if(endTile.piece != null || endTile.isDisabled){
                ReturnToHome();
            }
            else if(Vector2Int.Distance(homeTile.gridPosition, endTile.gridPosition) >= 3){ 
                ReturnToHome();
            }
            else{
                if(Vector2Int.Distance(homeTile.gridPosition, endTile.gridPosition) >= 2){
                    Hop();
                }
                else{
                    Spread();
                }
            }

        }
        void ReturnToHome(){
            Debug.Log(Vector2Int.Distance(homeTile.gridPosition, endTile.gridPosition));
            endTile = null;
            transform.position = homeTile.transform.position;
        }
        void Hop(){
            Debug.Log(Vector2Int.Distance(homeTile.gridPosition, endTile.gridPosition));
            homeTile.piece = null;
            homeTile = endTile;
            endTile = null;
            homeTile.piece = this;
            transform.position = homeTile.transform.position;
            Infect();
        }
        void Spread(){
            Debug.Log(Vector2Int.Distance(homeTile.gridPosition, endTile.gridPosition)) ;
            transform.position = homeTile.transform.position;
            var p = Instantiate(prefab, endTile.transform.position, Quaternion.identity);
            p.homeTile = endTile;
            endTile.piece = p;
            p.team = team;
            endTile = null;
            p.isPlayable = false;
            p.Infect();


        }
        void Infect(){
            foreach(var tile in homeTile.adjacentTiles){
                if(tile.piece == null)continue;
                tile.piece.ChangeTeam(team);
                tile.piece.isPlayable = false;
            }
            gm.EndTurn();
        }
        #endregion

        #region Inputs
        private void OnMouseEnter() {
            if(!isPlayable)return;
            animator.SetBool("isHovered", true);
        }

        private void OnMouseExit() {
            if(!isPlayable)return;
            animator.SetBool("isHovered", false);
        }

        private void OnMouseDown() {
            if(!isPlayable)return;
            player.selectedPiece = this;
            lr.SetPosition(0, transform.position);
            lr.enabled = true;
            animator.SetBool("isSelected", true);
        }

        private void OnMouseUp() {
            if(!isPlayable)return;  
            lr.enabled = false;
            animator.SetBool("isSelected", false);
            player.selectedPiece = null;
            endTile = player.hoveredTile;
            PieceMoved();

        }
        private void OnMouseDrag() {
            if(!isPlayable)return;
            lr.SetPosition(1, GetMousePosition());
            //transform.position = GetMousePosition();
        }

        Vector3 GetMousePosition(){
            var screenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return new Vector3(screenPosition.x, screenPosition.y, 5);
        }
        
        #endregion

    }

    public enum Team{
        RedTeam,
        GreenTeam
    }
}

