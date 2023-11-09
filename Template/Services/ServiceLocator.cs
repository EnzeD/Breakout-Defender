using MainSpace;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> listServices = new Dictionary<Type, object>();

    public static Health PlayerHealth { get; set; }
    public static Action OnLoseHealth;

    public static void RegisterService<T>(T service)
        {
            listServices[typeof(T)] = service;
        }

    public static T GetService<T>()
    {
        return (T)listServices[typeof(T)];
    }

    public static int Xp { get; set; }
    public static int Level { get; set; }
    public const int DIST_FROM_BOTTOM_SCREEN = 55;

    public static void InitializeHealth(Health health)
    {
        PlayerHealth = health;
    }
    public static void LoseHealth()
    {
        PlayerHealth.PlayerHealth -= 10;
        OnLoseHealth?.Invoke();
    }

    /* TO DO
     * score
     * improve ScreenInfo
     * xp
     * 
     */

}