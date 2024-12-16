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
    public bool isAIAutoMoveEnabled = true; // Attribut pour activer/désactiver le mode AI

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
    public delegate void GameOverEventHandler();  // (ajouté)
    public event GameOverEventHandler OnGameOver;  // (ajouté)
    
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
        // Ajouter l'appel de fonction requis pour la navigation automatique
        if (isAIAutoMoveEnabled)
        {
            CalculatePath();
        }

        //Question 4 : Gestion de l'évènement fin du jeu
        // Ajouter l'abonnement à l'événement OnGameOver
        OnGameOver += RestartGame;
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
            // Contrôler le joueur manuellement si le mode AI est désactivé
            if (!isAIAutoMoveEnabled)
            {
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");
                movement = new Vector2(h * speed, v * speed);
            }
            else
            {
                // Mode AI : Calcul du chemin et mouvement automatique
                UpdateAIMovement();
                CheckIfStuck(); // Vérifier si le joueur est bloqué et recalculer le chemin si nécessaire
            }
        }

        // Question 5 : Sauvegarde de la progression du jeu 
        if (ScoreManager.Instance.currentEnergy <= 0)
        {
        _isGameOver = true;
        Debug.Log("Game Over! No more energy.");
        // Appel pour sauvegarder le score dans MongoDB
        string playerName = "HadilLahar"; 
        int score = ScoreManager.Instance.playerScore;

        // Appelle la méthode asynchrone pour sauvegarder
        _ = PlayerData.SaveScoreAsync(playerName, score);
        // Ici, vous pouvez ajouter une logique pour afficher un écran de fin de jeu ou redémarrer la scène.
        OnGameOver?.Invoke();
        }
        // Vérifier si le joueur a franchi la ligne d'arrivée
        PlayerCrossedFlags();
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
    // Methode qui gère la fin du jeu
    void PlayerCrossedFlags()
    {
        if (rigidBody2D.position.x > 24.0f && !_isGameOver)
        {
            _isGameOver = true;
            Debug.Log("Reached the finish line!");
            // Question 4 : Déclenche l'événement de fin du jeu
            OnGameOver?.Invoke();  // (ajouté ici)
        }
    }

    // Fonction qui gère le redémarrage du jeu
    void RestartGame()  // (ajouté à la question 4)
    {
        Debug.Log("Restarting game...");
        // Logique de redémarrage du jeu, par exemple réinitialiser les positions, l'énergie, etc.
        _isGameOver = false;
        transform.position = new Vector2(0, 0); // Réinitialiser la position du joueur
        ScoreManager.Instance.ResetScore(); // Réinitialiser les scores
        path.Clear(); // Effacer le chemin actuel
        CalculatePath(); // Recalculer le chemin pour recommencer le jeu
    }
    void OnDestroy()
    {
        OnGameOver -= RestartGame;  // (ajouté ici)
    }
}
