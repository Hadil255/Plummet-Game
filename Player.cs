using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    // Vitesse de mouvement du joueur
    public float speed = 1.5f;
    private Rigidbody2D rigidBody2D;
    private Vector2 movement;
    private PlayerData playerData;
    private bool _isGameOver;
    
    // Question 6 : Ajout d'un mode AI  - attribut pour activer le mode AI

    private List<Vector2> path; //chemin de Djikstra
    private int currentPathIndex; 
    private float pathRecalculationTime = 1f; //frequence de recalcul du chemin
    private float lastPathCalculationTime; //duree de calcul du chemin
    private float stuckCheckTime = 3.5f; //frequence de detection de chemin bloque
    private float lastStuckCheckTime; //derniere fois ou la detection de chemin bloque a ete faite
    private Vector2 lastPosition; //derniere position du joueur
    private int stuckCounter = 0; //nombre de fois ou le chemin est bloque
    private float backtrackDistance = 2f; // distance de recul pour essayer un nouveau chemin

    // Références au ScoreManager pour gérer le score
    private ScoreManager scoreManager;

    //Question 4 : Gestion de l'évènement fin du jeu
    
    void Start()
    {
        _isGameOver = false;
        rigidBody2D = GetComponent<Rigidbody2D>();
        playerData = new PlayerData();
        playerData.plummie_tag = "nraboy";
        path = new List<Vector2>();
        lastPosition = transform.position;

        // Initialisation du ScoreManager
        scoreManager = ScoreManager.Instance;

        // Question 6 : Ajout d'un mode AI  - calcul du chemin
        // rajouter l'appel de fonction requis

        //Question 4 : Gestion de l'évènement fin du jeu
    }

    //methode qui calcule le chemin a prendre en Mode AI
    void CalculatePath()
    {
        if (Time.time - lastPathCalculationTime < pathRecalculationTime)
            return;

        lastPathCalculationTime = Time.time;
        Vector2 startPos = transform.position;
        
        // Add random vertical offset to try different paths
        float verticalOffset = Random.Range(-2f, 2f);
        Vector2 targetPos = new Vector2(25f, transform.position.y + verticalOffset);
        
        path = Dijkstra.FindPath(startPos, targetPos, stuckCounter);
        currentPathIndex = 0;
        stuckCounter = 0; // Reset stuck counter when finding new path
    }

    //methode pour verifier si le joueur est bloque en mode AI
    void CheckIfStuck()
    {
        if (Time.time - lastStuckCheckTime < stuckCheckTime)
            return;

        float distanceMoved = Vector2.Distance(transform.position, lastPosition);
        if (distanceMoved < 0.1f && !_isGameOver)
        {
            stuckCounter++;
            if (stuckCounter > 3) // If stuck for too long
            {
                BacktrackAndFindNewPath();
            }
        }
        else
        {
            stuckCounter = 0;
        }

        lastPosition = transform.position;
        lastStuckCheckTime = Time.time;
    }

    //retour en arriere et recherche d'un nouveau chemin
    void BacktrackAndFindNewPath()
    {
        // le mode AI fait reculer le joueur
        Vector2 backtrackPosition = transform.position;
        backtrackPosition.x -= backtrackDistance;
        transform.position = backtrackPosition;

        // le chemin actuel est efface et un nouveau chemin est recalcule
        path.Clear();
        currentPathIndex = 0;
        lastPathCalculationTime = 0f; // met a 0 le delai de temps pour le calcul du chemin
        CalculatePath();
    }

    void Update()
    {
        // Met à jour les scores en fonction des actions du joueur
        if (!_isGameOver)
        {
            // Détecter le mouvement du joueur
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            movement = new Vector2(h * speed, v * speed);

            // Ici tu peux ajouter des conditions pour contrôler les événements comme les collisions

            // Exemple : Si le joueur se déplace et entre en collision
            // Appeler ReduceEnergy et AddCollision ici en fonction des conditions
        }

        // Question 5 : Sauvegarde de la progression du jeu 
        if (ScoreManager.Instance.currentEnergy <= 0)
        {
        _isGameOver = true;
        Debug.Log("Game Over! No more energy.");
        // Ici, vous pouvez ajouter une logique pour afficher un écran de fin de jeu ou redémarrer la scène.
        }
    }

    //mouvement automatique controlle par le mode AI
    void UpdateAIMovement()
    {
        if (path == null || path.Count == 0 || currentPathIndex >= path.Count)
        {
            CalculatePath();
            return;
        }

        Vector2 currentTarget = path[currentPathIndex];
        Vector2 currentPosition = transform.position;
        Vector2 direction = (currentTarget - currentPosition);
        float distance = direction.magnitude;

        if (distance < 0.1f)
        {
            currentPathIndex++;
            if (currentPathIndex >= path.Count)
            {
                CalculatePath();
                return;
            }
        }

        movement = direction.normalized * speed;
    }
    
    // Vérification du progrès du joueur
    void FixedUpdate()
    {
        if (!_isGameOver)
        {
            Vector2 newPosition = rigidBody2D.position + movement * Time.fixedDeltaTime;
            rigidBody2D.MovePosition(newPosition);
        }

        if (rigidBody2D.position.x > 24.0f && _isGameOver == false)
        {
            _isGameOver = true;
            Debug.Log("Reached the finish line!");
        }
    }

    // Méthode qui s'exécute lors d'une collision
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall")) // Vérifie si l'objet est un mur
        {
        // Augmente le nombre de collisions dans le ScoreManager
        scoreManager.AddCollision();

        // Réduire l'énergie en fonction de la collision
        scoreManager.ReduceEnergy(50); // Par exemple, réduire 50 d'énergie à chaque collision

        // Retirer un mur (si applicable)
        scoreManager.RemoveWall();
        }
    }

    // Dessin et coloration des gizmos
    void OnDrawGizmos()
    {
        if (path != null && path.Count > 0)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(path[i], path[i + 1]);
            }
        }
    }

    // Question 4 : Gestion de l'évènement fin du jeu
}
