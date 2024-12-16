# Plummet-Game

***Description du jeu***
Le jeu consiste à guider un personnage (le joueur) à travers un environnement rempli de portes, certaines étant réelles et d'autres fausses. 
Le joueur doit naviguer en utilisant des mécanismes de mouvement basés sur l'algorithme de Dijkstra, 
qui calcule les chemins optimaux pour éviter les obstacles et atteindre un objectif. Dans certaines situations, le joueur peut être bloqué par des obstacles, 
et un système de rétrogradation permet de recalculer un nouveau chemin. 
Le jeu intègre un mode AI où le joueur peut être contrôlé par une intelligence artificielle qui analyse constamment son environnement pour trouver le chemin le plus court vers l'objectif, 
tout en évitant les fausses portes et autres pièges. 
La gestion des collisions et des obstacles fait partie intégrante du gameplay, ajoutant une couche de stratégie et de réflexion pour atteindre la victoire.

***Hiérarchie des classes***
Le jeu est structuré autour de plusieurs classes qui gèrent les différents aspects du gameplay, 
notamment le mouvement du joueur, l'intelligence artificielle, la gestion des portes et la caméra.
--------------------------------------1. CameraPosition---------------------------------------------------
Cette classe est responsable de suivre la position du joueur en ajustant la position de la caméra. 
Elle est simple et se limite à déplacer la caméra sur l'axe x et y en fonction de la position du joueur, mais avec une valeur constante sur l'axe z (fixe à -12).
Responsabilité principale : Maintenir la position de la caméra par rapport au joueur.
-------------------------------------2. Dijkstra------------------------------------------------------------------------------------------
La classe Dijkstra implémente l'algorithme de Dijkstra pour trouver le chemin optimal entre deux points sur une carte. 
Elle gère les obstacles, la direction du mouvement, et optimise le chemin en fonction de différents critères, comme la détection des situations où le joueur est bloqué (compteur stuckCounter).
Responsabilité principale : Calculer le chemin optimal pour le joueur en utilisant l'algorithme de Dijkstra.
-------------------------------------3.Player-----------------------------------------------------------------------------------------------
La classe Player représente le joueur dans le jeu. Elle gère les mouvements, l'intelligence artificielle (mode AI), 
ainsi que la logique liée aux collisions et à la fin du jeu. Elle interagit avec l'algorithme de Dijkstra pour déterminer les chemins à suivre en mode AI et met à jour la position du joueur à chaque itération.
Responsabilité principale : Contrôler le joueur, y compris les mouvements manuels et automatiques (mode AI).
-------------------------------------4.PlayerData------------------------------------------------------------------------------------------------------------------------------------------------------------
La classe PlayerData est responsable de la gestion des données du joueur, comme le nombre de collisions et d'étapes. 
Elle permet de sérialiser et désérialiser ces données pour la sauvegarde et le chargement.
Responsabilité principale : Gérer les données liées au joueur (collisions, étapes) et leur sérialisation.
-------------------------------------5. Door-------------------------------------------------------------------------------------------------------------------------------------------------
La classe Door gère l'état des portes dans le jeu. Une porte peut être vraie (avec une dynamique physique) ou une fausse porte (statique, sans effet physique). 
Elle contient les objets physiques des portes (haut et bas) et détermine leur comportement selon l'état de la porte.
Responsabilité principale : Gérer les portes (réelles ou fausses) et leur état physique.
---------------------------------6. DoorManager--------------------------------------------------------------------------------------------------------------------
Le DoorManager gère l'ensemble des portes dans une scène. Il attribue de manière aléatoire un certain nombre de fausses portes parmi toutes les portes existantes. 
Ce gestionnaire est responsable de l'initialisation de l'état des portes au début du jeu.
Responsabilité principale : Gérer la création et l'assignation des portes (vraies et fausses) dans le jeu.
