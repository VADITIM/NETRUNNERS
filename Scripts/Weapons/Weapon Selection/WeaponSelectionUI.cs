// using UnityEngine;
// using UnityEngine.UI;

// public class WeaponSelectionUI : MonoBehaviour
// {
//     [SerializeField] private Button[] weaponButtons;
//     [SerializeField] private string[] weaponNames;

//     private void Start()
//     {
//         for (int i = 0; i < weaponButtons.Length; i++)
//         {
//             int index = i;
//             weaponButtons[i].onClick.AddListener(() => SelectWeapon(weaponNames[index]));
//         }
//     }

//     private void SelectWeapon(string weaponName)
//     {
//         WeaponManager.SetSelectedWeapon(weaponName);
//     }
// }