# TP-2_DevApp

# TP 2 - Système de Gestion Pays/Provinces avec SQLAlchemy et PySide6

**Université :** [Nom de votre université]  
**Cours :** Programmation avec Base de Données  
**Durée :** 4 heures  
**Note :** / 100 points

---

## Objectifs Pédagogiques

À la fin de ce travail pratique, l'étudiant sera capable de :

1. **Concevoir** une base de données relationnelle avec contraintes d'intégrité
2. **Implémenter** des relations 1-à-N avec SQLAlchemy ORM
3. **Développer** une interface graphique avec PySide6
4. **Gérer** les listes de sélection dépendantes (ComboBox)
5. **Manipuler** les contraintes de clés étrangères
6. **Tester** la suppression en cascade
7. **Valider** l'intégrité des données

---

## Contexte du Projet

Vous devez développer un **système de gestion géographique** permettant de gérer des pays et leurs provinces/états. Le système doit respecter les contraintes suivantes :

- Un **pays** peut avoir plusieurs **provinces**
- Une **province** appartient obligatoirement à un **pays**
- Impossible d'ajouter une province sans pays parent
- La suppression d'un pays supprime automatiquement ses provinces

---

## Spécifications Techniques

### 1. Base de Données (25 points)

#### Table `pays`
| Colonne    | Type         | Contraintes                    |
|------------|--------------|--------------------------------|
| id         | Integer      | Clé primaire, auto-increment   |
| nom        | String(100)  | Unique, non null               |
| code_iso   | String(3)    | Unique, non null               |

#### Table `provinces`
| Colonne        | Type         | Contraintes                    |
|----------------|--------------|--------------------------------|
| id             | Integer      | Clé primaire, auto-increment   |
| nom            | String(100)  | Non null                       |
| code_province  | String(10)   | Non null                       |
| pays_id        | Integer      | Clé étrangère vers pays.id     |

#### Contraintes Supplémentaires
- **Contrainte unique** : `(nom, pays_id)` pour éviter les doublons de provinces dans un même pays
- **Cascade de suppression** : `cascade="all, delete-orphan"`
- **Relation bidirectionnelle** entre pays et provinces

### 2. Données de Test Obligatoires (10 points)

Votre application doit initialiser automatiquement ces données :

#### Pays
```
Canada (CAN)
États-Unis (USA)
France (FRA)
Allemagne (DEU)
Brésil (BRA)
```

#### Provinces/États
```
Canada : Québec (QC), Ontario (ON), Alberta (AB), Colombie-Britannique (BC)
États-Unis : New York (NY), Californie (CA), Texas (TX), Floride (FL)
France : Île-de-France (IDF), Provence-Alpes-Côte d'Azur (PACA), Nouvelle-Aquitaine (NAQ)
Allemagne : Bavière (BY), Bade-Wurtemberg (BW), Rhénanie-du-Nord-Westphalie (NW)
Brésil : Saint Paul (SP)
```

### 3. Interface Graphique (35 points)

#### Composants Requis

1. **Zone de Sélection** (15 points)
   - `QComboBox` pour les pays
   - `QComboBox` pour les provinces (mise à jour automatique)
   - `QPushButton` "Afficher la Sélection" (activé seulement si les deux sélections sont faites)

2. **Zone d'Ajout Pays** (10 points)
   - `QLineEdit` pour le nom du pays
   - `QLineEdit` pour le code ISO (3 caractères)
   - `QPushButton` "Ajouter Pays"

3. **Zone d'Ajout Province** (10 points)
   - `QLineEdit` pour le nom de la province
   - `QLineEdit` pour le code province
   - `QComboBox` pour sélectionner le pays parent
   - `QPushButton` "Ajouter Province"

4. **Zone d'Affichage**
   - `QTextEdit` pour afficher les résultats et messages

#### Disposition Suggérée
```
┌─────────────────────────────────────────────────────────┐
│                  Système Pays/Provinces                 │
├─────────────────────────────────────────────────────────┤
│ Sélection :                                             │
│ Pays: [ComboBox▼] Provinces: [ComboBox▼] [Afficher]    │
├─────────────────────────────────────────────────────────┤
│ Ajouter un Pays :                                       │
│ Nom: [_____________] Code ISO: [___] [Ajouter Pays]     │
├─────────────────────────────────────────────────────────┤
│ Ajouter une Province :                                  │
│ Nom: [_____________] Code: [_____] Pays: [ComboBox▼]    │
│ [Ajouter Province]                                      │
├─────────────────────────────────────────────────────────┤
│ Résultats :                                             │
│ ┌─────────────────────────────────────────────────────┐ │
│ │                                                     │ │
│ │           Zone d'affichage des résultats            │ │
│ │                                                     │ │
│ └─────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────┘
```

### 4. Fonctionnalités Métier (30 points)

#### A. Listes Dépendantes (15 points)
- Chargement initial de tous les pays dans le premier ComboBox
- Mise à jour automatique des provinces selon le pays sélectionné
- Gestion du cas "aucun pays sélectionné" (provinces vides)
- Activation/désactivation du bouton "Afficher la Sélection"

#### B. Ajout de Données (10 points)
- **Ajout de pays** : validation des champs obligatoires
- **Ajout de province** : vérification de l'existence du pays parent
- **Gestion des erreurs** : doublons, champs vides, contraintes violées
- **Mise à jour des ComboBox** après ajout

#### C. Validation et Intégrité (5 points)
- Messages d'erreur clairs et informatifs
- Nettoyage des champs après ajout réussi
- Gestion des exceptions SQLAlchemy

---

## Structure de Fichiers Requise

```
tp2_pays_provinces/
├── database.py          # Modèles SQLAlchemy + initialisation
├── interface.py         # Interface PySide6 (écrite manuellement)
├── main.py             # Application principale + logique métier
├── demo.py             # Script de test et démonstration
├── requirements.txt    # Dépendances
└── README.md           # Documentation
```

---

## Critères d'Évaluation

### 1. Base de Données (25/100)
- **Modèles SQLAlchemy corrects** (10 pts)
- **Relations et contraintes** (10 pts)
- **Données d'initialisation** (5 pts)

### 2. Interface Graphique (35/100)
- **Disposition et composants** (15 pts)
- **Ergonomie et lisibilité** (10 pts)
- **Code interface propre** (10 pts)

### 3. Fonctionnalités (30/100)
- **Listes dépendantes fonctionnelles** (15 pts)
- **Ajout de données** (10 pts)
- **Gestion des erreurs** (5 pts)

### 4. Code et Documentation (10/100)
- **Qualité du code** (5 pts)
- **Documentation et commentaires** (3 pts)
- **Respect de la structure** (2 pts)

---

## Tests de Validation Obligatoires

Votre application doit réussir ces tests :

### Test 1 : Chargement Initial
```
✓ Application démarre sans erreur
✓ ComboBox pays contient 5 pays
✓ ComboBox provinces est vide initialement
✓ Bouton "Afficher" est désactivé
```

### Test 2 : Sélection Dépendante
```
✓ Sélection "Canada" → 4 provinces apparaissent
✓ Sélection "États-Unis" → 4 états apparaissent
✓ Sélection "France" → 3 régions apparaissent
✓ Bouton "Afficher" s'active après double sélection
```

### Test 3 : Ajout de Pays
```
✓ Ajout "Italie, ITA" → succès
✓ Ajout pays avec nom vide → erreur
✓ Ajout pays avec code ISO existant → erreur
✓ ComboBox pays mis à jour après ajout
```

### Test 4 : Ajout de Province
```
✓ Ajout "Lombardie, LOM" pour "Italie" → succès
✓ Ajout province sans pays sélectionné → erreur
✓ Ajout province en doublon → erreur
✓ ComboBox provinces mis à jour
```

### Test 5 : Contrainte d'Intégrité
```
✓ Impossible d'ajouter province pour pays inexistant
✓ Suppression d'un pays supprime ses provinces
✓ Messages d'erreur appropriés
```

---

## Consignes de Remise

### Format de Remise
- **Archive ZIP** nommée : `TP2_NomPrenom.zip`
- **Tous les fichiers** `.py` et `.txt` requis
- **README.md** avec instructions d'installation et d'utilisation

### Date Limite
**[Date à préciser]** à **23h59**

### Modalités
- Travail **individuel** obligatoire
- Code doit être **original** (pas de copie)
- **Démonstration** en laboratoire la semaine suivante

---

## Ressources Autorisées

- Documentation officielle SQLAlchemy
- Documentation officielle PySide6
- Cours et notes de laboratoire
- Recherche internet pour syntaxe spécifique

### Ressources Interdites
- Code source complet provenant d'autres étudiants
- Solutions complètes trouvées en ligne
- Assistance directe d'une autre personne

---

## Aide et Support

### Problèmes Techniques
- **Installation** : Consulter `INSTALLATION.md`
- **SQLite** : Consulter `03_GUIDE_SQLITE_COMMANDES.md`
- **Exemples** : Voir dossier `evaluation/` pour inspiration

### Contact
- **Professeur** : [email@université.ca]
- **Heures de bureau** : [Horaires]
- **Forum de discussion** : [Lien plateforme]

---

## Bonus (5 points supplémentaires)

### Options Avancées
- **Export CSV** des données complètes
- **Interface esthétique** avec styles CSS
- **Recherche/filtre** dans les ComboBox
- **Statistiques** (nombre de provinces par pays)

---

## Exemple de Session d'Utilisation

```
1. Démarrage de l'application
   → Interface s'ouvre avec données préchargées

2. Test de sélection
   → Sélectionner "Canada" → voir "Québec, Ontario, Alberta, Colombie-Britannique"
   → Sélectionner "Québec" → bouton "Afficher" activé
   → Clic "Afficher" → message "Sélection : Canada - Québec (QC)"

3. Ajout d'un pays
   → Saisir "Italie" et "ITA"
   → Clic "Ajouter Pays" → message "Pays ajouté avec succès"
   → "Italie" apparaît dans les ComboBox

4. Ajout d'une province
   → Saisir "Lombardie", "LOM", sélectionner "Italie"
   → Clic "Ajouter Province" → message "Province ajoutée avec succès"
   → "Lombardie" disponible quand "Italie" est sélectionné

5. Test d'erreur
   → Tenter ajouter pays avec nom vide
   → Message d'erreur approprié affiché
```

---

## Grille d'Évaluation Détaillée

| Critère                           | Points | Description                                    |
|-----------------------------------|--------|------------------------------------------------|
| **Modèle Pays**                   | 5      | Attributs corrects, contraintes, relations    |
| **Modèle Province**               | 5      | Attributs corrects, clé étrangère, contraintes|
| **Relations SQLAlchemy**          | 5      | Bidirectionnelle, cascade, back_populates     |
| **Initialisation données**        | 5      | Fonction de seed, données complètes           |
| **Interface - Disposition**       | 8      | Layout cohérent, composants bien placés       |
| **Interface - ComboBox**          | 8      | Fonctionnement correct, mise à jour           |
| **Interface - Boutons/Champs**    | 7      | Validation, états activé/désactivé            |
| **Interface - Zone affichage**    | 5      | Messages clairs, formatage lisible            |
| **Interface - Code qualité**      | 7      | Structure propre, nommage cohérent            |
| **Logique - Chargement initial**  | 5      | Données chargées correctement                  |
| **Logique - Listes dépendantes**  | 10     | Mise à jour automatique fonctionnelle         |
| **Logique - Ajout pays**          | 5      | Validation, gestion erreurs                    |
| **Logique - Ajout provinces**     | 5      | Contraintes respectées, validation            |
| **Gestion erreurs**               | 5      | Try/catch appropriés, messages utilisateur    |
| **Code général**                  | 5      | Lisibilité, commentaires, structure           |
| **Documentation**                 | 3      | README complet, instructions claires          |
| **Respect structure fichiers**    | 2      | Noms et organisation respectés                |
| **TOTAL**                         | **100**| **Note finale**                               |

---

**Bonne chance dans votre travail pratique !**

**Date de création :** [Date actuelle]  
**Version :** 1.0  
**Auteur :** [Nom du professeur]

https://github.com/hrhouma1/Pyside6_Students_ORM_3/blob/main/04_ENONCE_TP2_OFFICIEL.md