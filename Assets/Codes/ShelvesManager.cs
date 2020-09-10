using System;
using UnityEngine;
public class ShelvesManager : MonoBehaviour
{
    [SerializeField] private GameObject[] defaultBottles = null;
    [SerializeField] private GameObject shelvesGridObject = null;
    private GameObject[][] gridObjects = null;
    private GameObject[] shelves = null;
    private int refBottlesLength = 0;
    private float horizontal = 0;
    private float bottleHorizontalSize = 0;

    //Bu script bottleParent objesinde bulunur. Raf objesine ait gridin ve raflarda bulunan şişelerin oluşturulmasını sağlar.
    private void Start()
    {
        //Ekran ölçülerine göre bulunan değişkenlerdeki değerler bu scripte ait değişkenlere atanır.
        refBottlesLength = CreateManager.createManagerClass.refBottlesLength;
        horizontal = CreateManager.createManagerClass.horizontal;
        bottleHorizontalSize = CreateManager.createManagerClass.bottleHorizontalSize;

        //Her bir rafta bulunan ve pozisyon değerini referans alacağımız shelves tagine ait objeler bulunur.
        shelves = GameObject.FindGameObjectsWithTag("shelves");

        SortShelvesPosY();
        CreateShelvesGrid();
        PlaceDefaultBottles();
    }
    private void SortShelvesPosY()
    {
        //shelves tagine ait objelerin pozisyon.y değerleri büyükten küçüğe sıralanır. 
        for (int i = 0; i < shelves.Length - 1; i++)
        {
            for (int j = 0; j < shelves.Length - 1; j++)
            {
                if (shelves[j].transform.position.y < shelves[j + 1].transform.position.y)
                {
                    GameObject temp = shelves[j];
                    shelves[j] = shelves[j + 1];
                    shelves[j + 1] = temp;
                }
            }
        }
        //Sıralama işlemi bittikten sonra dizinin ilk elemanı en üstteki rafa ait referans objesidir.
    }
    private void CreateShelvesGrid()
    {
        gridObjects = new GameObject[shelves.Length][];
        //Referans alınan grid objesini oluşturur.
        for (int i = 0; i < shelves.Length; i++)
        {
            gridObjects[i] = new GameObject[refBottlesLength];

            for (int j = 0; j < refBottlesLength; j++)
            {
                GameObject shelvesGridObjectCopy = Instantiate(shelvesGridObject, shelves[i].transform.parent);
                //shelvesGridObjectCopy.transform.parent = shelves[i].transform.parent;
                //Boyut ayarları ekran ölçülerine ve şişe sayısına göre yapılır.
                Vector3 shelvesGridObjectCopyScale = (shelvesGridObjectCopy.transform.localScale * horizontal);
                shelvesGridObjectCopyScale.x /= refBottlesLength;
                shelvesGridObjectCopy.transform.localScale = shelvesGridObjectCopyScale;

                //Pozisyon ayarları ekran ölçülerine göre ve shelves dizisindeki objelerin pozisyonlarına göre yapılır.
                Vector3 shelvesGridObjectCopyPosition = shelves[i].transform.position;
                shelvesGridObjectCopyPosition.x = (j * bottleHorizontalSize) - horizontal + (horizontal / refBottlesLength) + 10;
                shelvesGridObjectCopy.transform.position = shelvesGridObjectCopyPosition;
                gridObjects[i][j] = shelvesGridObjectCopy;

                switch (i)
                {
                    //En üstteki rafta bulunan grid objeleri "bottle3", ortadaki "bottle2", en alttaki ise "bottle1" taglerini alır.
                    case 0:
                        shelvesGridObjectCopy.tag = "bottle3";
                        break;
                    case 1:
                        shelvesGridObjectCopy.tag = "bottle2";
                        break;
                    case 2:
                        shelvesGridObjectCopy.tag = "bottle1";
                        break;
                }
            }
            //İlgili satırdaki tüm grid objeler oluşturulduktan sonra pozisyon.y değerini referans aldığımız obje sahneden kaldırılır.
            Destroy(shelves[i]);
        }
    }
    private void PlaceDefaultBottles()
    {
        //Editörden alınan defaultBottles dizisindeki şişeler rastgele raflarda oluşturulur.
        for (int i = 0; i < defaultBottles.Length; i++)
            SetPos(Instantiate(defaultBottles[i]));
    }
    private void SetPos(GameObject refObject)
    {
        //Referans alınan şişenin tagine göre satır indisi bulunur.
        int i;
        for (i = 0; i < gridObjects.Length; i++)
        {
            if(gridObjects[i].Length > 0 && refObject.tag == gridObjects[i][0].tag)
                break;
        }
        //İlgili satırda rastgele bir sütuna şişe yerleştirilir.
        int index = UnityEngine.Random.Range(0, gridObjects[i].Length);
        refObject.transform.position = gridObjects[i][index].transform.position;
        refObject.transform.parent = gridObjects[i][index].transform.parent;
        refObject.transform.localScale *= (bottleHorizontalSize / 2);

        //Referans alınan grid objesi sahneden kaldırılır.
        //Böylelikle bu satır ve sütun değerine başka bir şişe atanamaz, koyulamaz.
        gridObjects[i][index].tag = "destroyedObject";
        Destroy(gridObjects[i][index]);

        for (int j = index; j < gridObjects[i].Length - 1; j++)
            gridObjects[i][j] = gridObjects[i][j + 1];

        Array.Resize(ref gridObjects[i], gridObjects[i].Length - 1);
    }
}