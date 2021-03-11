using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;


public class BoardManager : MonoBehaviour
{
	[Serializable]

	public class Count
	{
		public int minimum;
		public int maximum;

		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}

	public int columns = 8;
	public int rows = 8;
	public Count wallCount = new Count(5,9); //Indica que puede haber un minimo de 5 paredes por nivel y un maixmo de 9
	public Count foodCount = new Count(1,5); //Lo mismo pero con la comida
	public GameObject exit;
	public GameObject[] floorTiles;
	public GameObject[] wallTiles;
	public GameObject[] foodTiles;
	public GameObject[] enemyTiles;
	public GameObject[] outerWallTiles;

	private Transform boardHolder; // para mantener limpia la jerarquia
	private List <Vector3> gridPositions = new List<Vector3>();

	void InitialiseList() 
	{
		gridPositions.Clear();

		for (int x = 1; x < columns -1; x++)
		{
			for (int y = 1; y < rows -1; y++)
			{
				gridPositions.Add(new Vector3(x,y,0f));
			}
		}
	}

	void BoardSetup() //Rellenar el suelo y la pared
	{
		boardHolder = new GameObject ("Board").transform;

		for (int x=-1; x<columns +1; x++)
		{
			for (int y = -1; y< rows +1; y++)
			{
				GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)]; //Introduce un objeto aleatoriamente del vector de suelos.

				if (x==-1|| x==columns || y==-1 || y==rows) //Mira si estamos en alguna posicion donde deberia haber una pared, si es asi pondra ahi una pared.
				{
					toInstantiate = outerWallTiles[Random.Range (0,outerWallTiles.Length)];
				}

				GameObject instance= Instantiate(toInstantiate, new Vector3 (x,y,0f), Quaternion.identity) as GameObject; //Introduce el objeto

				instance.transform.SetParent(boardHolder);
			}
		}
	}

	Vector3 RandomPosition() //Buscar una posicion random para poner el objeto
	{
		int randomIndex = Random.Range(0, gridPositions.Count); //Escoger valor aleatorio
		Vector3 RandomPosition = gridPositions[randomIndex]; //Crea un vector igual al de la posicion elegida
		gridPositions.RemoveAt(randomIndex); //Eliminar la posicion para no duplicar objetos en la misma posicion.
		return RandomPosition;

	}

	 void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
    {
    	int objectCount = Random.Range (minimum, maximum+1); //Determina el numero de objetos en un nivel aleatoriamente entre el minimo y maximo
    	for(int i = 0; i < objectCount; i++)
    	{
    		Vector3 randomPosition = RandomPosition(); //Para utilizar la posicion random de la funcion anterior
    		GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)]; //Elegir objeto aleatorio
    		Instantiate(tileChoice, randomPosition, Quaternion.identity);

    	}
    }

    public void SetupScene(int level)
    {
    	BoardSetup();
    	InitialiseList();
    	LayoutObjectAtRandom(wallTiles,wallCount.minimum, wallCount.maximum);
    	LayoutObjectAtRandom(foodTiles,foodCount.minimum, foodCount.maximum);
    	int enemyCount = (int)Mathf.Log(level,2f); //crear los enemigos de forma expoonencial
    	LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
    	Instantiate(exit, new Vector3(columns-1,rows-1, 0f), Quaternion.identity);
    }

}
