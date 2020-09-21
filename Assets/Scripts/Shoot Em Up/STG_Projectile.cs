using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// @brief Projektiililuokka Shoot Em Up -peliä varten.
/// @details Yleisesti käytettävä luokka pelissä, joka liikuttaa projektiilia 2D-akselistolla "eteenpäin", eli lokaalilla X-akselilla positiiviseen suuntaan.
public class STG_Projectile : MonoBehaviour {
    /// @details Nopeusmuuttuja liikkumismetodia varten.
    [SerializeField] private int speed;

	void Update () {
        Move();
	}

    /// @details Liikkumismetodi.
    void Move() {
        transform.position = transform.position + transform.right * speed * Time.deltaTime;
    }
}
