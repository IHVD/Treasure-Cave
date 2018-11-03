﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	public Player player;
	public float jumpForce = 10f;
	public GameObject linePrefab;
	public Rigidbody2D playerRb;
	public DistanceJoint2D playerJoint;
	public CameraMover cameraMover;
	public float maxRopeLength;
	public float climbSpeed = 2f;
	private GameObject currentLine;
	
	public Vector2 maxVelocity = new Vector2(10f, 100000f);
    public float slowdownSpeed = 25f;

    void Update () {
		if(Input.GetMouseButtonUp(0)){
			Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3 dir = (playerRb.transform.position - position).normalized;
			RaycastHit2D hit = Physics2D.Raycast(playerRb.transform.position, -dir);
			Debug.DrawRay(playerRb.transform.position, dir * 10f, Color.green);
			if (hit.collider != null){
				Debug.Log("hit");
				GameObject newLine = Instantiate(linePrefab, new Vector3(0f, 0f, -2f) + new Vector3(hit.point.x, hit.point.y), Quaternion.identity);
				newLine.GetComponent<Line>().target = playerJoint.transform;
				playerJoint.autoConfigureDistance = true;
				playerJoint.connectedBody = newLine.GetComponent<Rigidbody2D>();

				if(currentLine == null){ //start player
					playerRb.simulated = true;
					currentLine = newLine;
				}else{
					Destroy(currentLine);
					currentLine = newLine;
				}

				maxRopeLength = hit.point.y * 1.5F;
				playerRb.velocity = playerRb.velocity + Vector2.right * playerJoint.distance * jumpForce;
				cameraMover.targetyX = hit.point.x;
				cameraMover.moveSpeed = playerJoint.distance;
				player.rope = newLine.transform;
			}

			
		}
	}

	private void FixedUpdate() {
		if(playerJoint.distance != maxRopeLength){
			playerJoint.autoConfigureDistance = false;
			playerJoint.distance = Mathf.MoveTowards(playerJoint.distance, maxRopeLength, Time.deltaTime * climbSpeed);
		}

		playerRb.velocity = Vector2.Lerp(playerRb.velocity, new Vector2(Mathf.Clamp(playerRb.velocity.x, -maxVelocity.x, maxVelocity.x), playerRb.velocity.y), slowdownSpeed / (playerJoint.distance) * Time.deltaTime);
	}
}
