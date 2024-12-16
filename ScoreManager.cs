using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    // Instance statique du singleton
    public static ScoreManager Instance;

    // Variables pour le score
    public int maxEnergy = 1000;
    public int currentEnergy;
    public int collisionCount;
    public int remainingWalls;

    // Références aux éléments UI pour afficher le score
    public Text energyText;
    public Text collisionText;
    public Text wallText;

    // Initialisation du ScoreManager
    private void Awake()
    {
        // Assurer qu'il n'y ait qu'une seule instance de ScoreManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optionnel : empêche la destruction de l'objet lors du changement de scène
        }
        else
        {
            Destroy(gameObject); // Détruit l'instance supplémentaire si elle existe déjà
        }
    }

    private void Start()
    {
        // Initialiser les valeurs
        currentEnergy = maxEnergy;
        collisionCount = 0;
        remainingWalls = 10; // Ajuste selon ton jeu (le nombre de murs)
        
        // Met à jour l'affichage initial
        UpdateUI();
    }

    // Méthode pour réduire l'énergie après une collision
    public void ReduceEnergy(int amount)
    {
        currentEnergy -= amount;
        if (currentEnergy < 0) currentEnergy = 0;
        UpdateUI();
    }

    // Méthode pour ajouter une collision
    public void AddCollision()
    {
        collisionCount++;
        UpdateUI();
    }

    // Méthode pour diminuer le nombre de murs restants
    public void RemoveWall()
    {
        remainingWalls--;
        if (remainingWalls < 0) remainingWalls = 0;
        UpdateUI();
    }

    // Méthode pour mettre à jour l'affichage de l'UI
    private void UpdateUI()
    {
        // Met à jour les valeurs de l'UI
        energyText.text = "Energy: " + currentEnergy;
        collisionText.text = "Collisions: " + collisionCount;
        wallText.text = "Remaining Walls: " + remainingWalls;
    }
}
