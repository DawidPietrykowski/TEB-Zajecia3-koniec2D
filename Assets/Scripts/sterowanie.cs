using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sterowanie : MonoBehaviour
{
    // Prędkość poruszania się
    public float predkosc = 10;
    
    // Siła skoku
    public float silaSkoku = 300;
    
    // Wynik gry
    public int wynik = 0;
    
    // Prefab (wzorzec poziomu)
    public GameObject prefabPoziom1;
    
    // Aktualny obiekt na scenie zawierający poziom
    public GameObject aktualnyPoziom;

    public LayerMask maskaWykrywaniaPodloza;

    public AudioSource dzwiekPrzegranej;

    public AudioSource dzwiekZebranegoPunktu;
    
    void Update()
    {
        // Pobieramy ruch od użytkownika w kierunku prawo/lewo
        float zmianaX = Input.GetAxisRaw("Horizontal");

        // Dodajemy sile o tej wielkosci pomnożonej przez prędkość
        GetComponent<Rigidbody2D>().AddForce(new Vector2(zmianaX * predkosc, 0));

        bool czyJestNaZiemi = Physics2D.BoxCast(
            new Vector2(transform.position.x, transform.position.y) + Vector2.down * 0.5f,
            new Vector2(0.1f, 0.1f), 0, Vector2.down, 0.1f, maskaWykrywaniaPodloza);
        
        Debug.Log("Czy jest na ziemi: " + czyJestNaZiemi);
        
        // Jeśli kliknięta spacja to dodajemy siłe w górę - skok
        if (Input.GetKeyDown(KeyCode.Space) && czyJestNaZiemi == true)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, silaSkoku));
            GetComponent<Animator>().SetBool("isOnGround", false);
        }

        // Jeśli gracz jest na wysokości poniżej -5 to znaczy że spadł
        if (transform.position.y < -5)
        {
            Debug.Log("Przegrana");
            
            // Usuwamy obiekt który zawiera aktualny poziom
            Destroy(aktualnyPoziom);
            
            // Tworzymy nowy poziom na podstawie prefabu i aktualizujemy zmienną aktualnyPoziom
            aktualnyPoziom = Instantiate(prefabPoziom1);
            
            // Resetujemy pozycję gracza do konkrentych wartości
            transform.position = new Vector3(-2, 1, 0);
            
            // Resetujemy prędkość gracza do 0
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            
            // Resetujemy wynik
            wynik = 0;
            
            // Puszczamy dźwięk przegranej
            dzwiekPrzegranej.Play();
        }

        float predkoscX = GetComponent<Rigidbody2D>().velocity.x;
        float predkoscXCalkowita = Mathf.Abs(predkoscX);
        GetComponent<Animator>().SetFloat("speed", predkoscXCalkowita);

        if (predkoscX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        } else if (predkoscX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    // Wywoływane w momencie zderzenia sie z punktami
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Usuwamy obiekt punktu
        Destroy(other.gameObject);
        
        wynik = wynik + 10;
        
        // Wypisujemy nowy wynik
        Debug.Log("Nowy wynik: " + wynik);
        
        // W momencie zebrania punktu puszczamy dźwięk
        dzwiekZebranegoPunktu.Play();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        GetComponent<Animator>().SetBool("isOnGround", true);
    }
}
