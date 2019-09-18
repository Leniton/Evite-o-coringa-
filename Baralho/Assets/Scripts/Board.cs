﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Board : MonoBehaviour
{
    string EndText;
    [SerializeField] GameObject endScreen;
    [SerializeField] int CardLimit;
    [SerializeField] Vector3[] places;
    [Space]
    [SerializeField] GameObject TurnHand, OtherHand;
    [Space]
    [SerializeField] Hand Player1, Player2;
    [SerializeField] TextMeshProUGUI seusPontos;
    [SerializeField] TextMeshProUGUI outroPontos;
    Sprite backImage;
    [Space]
    [SerializeField] Button continuar;
    [Space]
    [SerializeField] Animator cortina;
    [Space]
    [Space]
    [SerializeField] GameObject ChangeHand;
    [SerializeField] GameObject selectPrefab;
    [SerializeField] GameObject selected;
    [SerializeField] List<int> modHand;

    void Start()
    {
        backImage = OtherHand.transform.GetComponentInChildren<Image>().sprite;
        Transform[] a;
        a = OtherHand.GetComponentsInChildren<Transform>();
        places = new Vector3[a.Length - 1];
        for (int i = 0; i < places.Length; i++)
        {
            places[i] = a[i + 1].position;
        }

        Player1.GetCard(0);
        for (int i = 1; i < CardLimit; i++)
        {
            /*int card;
            do
            {
                card = Random.Range(1, Cardlist.cardlist.list.Length);
            } while (Player1.cards.Contains(card));
            */
            Player1.GetCard(Draw(Player1.cards));
        }

        Player2.GetCard(0);
        for (int i = 1; i < CardLimit; i++)
        {
            /*int card;
            do
            {
                card = Random.Range(1, Cardlist.cardlist.list.Length);
            } while (Player2.cards.Contains(card));
            */
            Player2.GetCard(Draw(Player2.cards));
        }

        flip();

        for (int i = 0; i < Player1.cards.Count; i++)
        {
            ChangeHand.transform.GetChild(i).GetComponent<Image>().sprite = Cardlist.cardlist.list[Player1.cards[i]];
        }
        modHand = Player1.cards;
        if (Player2.myTurn)
        {
            for (int i = 0; i < Player2.cards.Count; i++)
            {
                ChangeHand.transform.GetChild(i).GetComponent<Image>().sprite = Cardlist.cardlist.list[Player2.cards[i]];
            }
            modHand = Player2.cards;
        }
    }

    int Draw(List<int> b)
    {
        int n;
        do
        {
            n = Random.Range(1, Cardlist.cardlist.list.Length);
        } while (b.Contains(n));
        print("draw: " + n);
        return n;
    }

    private void flip()
    {
        if(TurnHand.transform.childCount < CardLimit)
        {
            Debug.LogError("o limite de cartas é maior que a quantidade de cartas!!");
            return;
        }

        Image[] s = TurnHand.GetComponentsInChildren<Image>();
        List<int> l;
        if (Player1.myTurn)
        {
            l = Player1.cards;
            seusPontos.text = "Sua pontuação: " + Player1.points;
        }
        else
        {
            l = Player2.cards;
            seusPontos.text = "Sua pontuação: " + Player2.points;
        }

        for (int i = 0; i < s.Length; i++)
        {
            s[i].sprite = Cardlist.cardlist.list[l[i]];
        }

        //se tiver alguma carta da outra mão exposta

        Image[] OM = OtherHand.GetComponentsInChildren<Image>();

        for (int i = 0; i < OM.Length; i++)
        {
            OM[i].GetComponent<Animator>().enabled = false;
            if(OM[i].sprite != backImage)
            {
                OM[i].sprite = backImage;
            }
            if (!OM[i].gameObject.GetComponent<Button>().interactable)
            {
                OM[i].gameObject.GetComponent<Button>().interactable = true;
            }
        }
    }

    public void pickCard()
    {
        GameObject carta = null;
        int index = 0;

        List<int> oh;
        if (Player1.myTurn)
        {
            oh = Player2.cards;
        }
        else
        {
            oh = Player1.cards;
        }

        for (int i = 0; i < OtherHand.transform.childCount; i++)
        {
            if (OtherHand.transform.GetChild(i).GetComponent<Animator>().enabled)
            {
                carta = OtherHand.transform.GetChild(i).gameObject;
                index = i;
            }
            OtherHand.transform.GetChild(i).GetComponent<Button>().interactable = false;
        }
        if(carta != null)
        {
            carta.GetComponent<Image>().sprite = Cardlist.cardlist.list[oh[index]];
        }

        //devolve qualquer modificação
        if (Player1.myTurn)
        {
            if (oh[index] != 0)
            {
                if (Player1.cards.Contains(oh[index]))
                {
                    Player1.points++;
                    if(oh[index] == 1)
                    {
                        Player1.points++;
                    }
                    if (Player1.points >= 10)
                    {
                        print("Vitoria!!!!");
                        EndText = "Você venceu!!!";
                        StartCoroutine("TheEnd");
                    }
                    Player1.cards.RemoveAt( Player1.cards.IndexOf(oh[index]) );
                    Player1.cards.Add(Draw(Player1.cards));
                }
                oh.RemoveAt(index);
                oh.Add(Draw(oh));
                seusPontos.text = "Sua pontuação: " + Player1.points;
                //sincronizar com o modificador

                for (int i = 0; i < oh.Count; i++)
                {
                    ChangeHand.transform.GetChild(i).GetComponent<Image>().sprite = Cardlist.cardlist.list[Player1.cards[i]];
                }
                modHand = Player1.cards;
            }
            else
            {
                print("derrota");
                EndText = "Você Perdeu...";
                StartCoroutine("TheEnd");
            }
            Player2.cards = oh;
        }
        else
        {
            if (oh[index] != 0)
            {
                if (Player2.cards.Contains(oh[index]))
                {
                    Player2.points++;
                    if (oh[index] == 1)
                    {
                        Player2.points++;
                    }
                    if(Player2.points >= 10)
                    {
                        print("Vitoria!!!!");
                        EndText = "Você venceu!!!";
                        StartCoroutine("TheEnd");
                    }
                    Player2.cards.RemoveAt( Player2.cards.IndexOf(oh[index]) );
                    Player2.cards.Add(Draw(Player2.cards));
                }
                oh.RemoveAt(index);
                oh.Add(Draw(oh));
                seusPontos.text = "Sua pontuação: " + Player2.points;
                //sincronizar com o modificador

                for (int i = 0; i < oh.Count; i++)
                {
                    ChangeHand.transform.GetChild(i).GetComponent<Image>().sprite = Cardlist.cardlist.list[Player2.cards[i]];
                }
                modHand = Player2.cards;
            }
            else
            {
                print("derrota");
                EndText = "Você Perdeu...";
                StartCoroutine("TheEnd");
            }
            Player1.cards = oh;
        }

        continuar.gameObject.SetActive(true);
    }

    public void DescerCortina()
    {
        if (!cortina.enabled)
        {
            cortina.enabled = true;
        }
        else
        {
            cortina.SetTrigger("T");
        }
    }

    public void SwitchTurn()
    {
        Player1.myTurn = !Player1.myTurn;
        Player2.myTurn = !Player2.myTurn;
        flip();
    }

    public void MoveCards(GameObject s)
    {
        if (selected != null)
        {
            selectPrefab.SetActive(false);
            Vector3 Pholder = selected.transform.position;
            selected.transform.position = s.transform.position;
            s.transform.position = Pholder;

            int fst, sec;
            fst = selected.transform.GetSiblingIndex();
            sec = s.transform.GetSiblingIndex();
            selected.transform.SetSiblingIndex(sec);
            s.transform.SetSiblingIndex(fst);

            int v,Sv;
            v = modHand[fst];
            Sv = modHand[sec];
            modHand.Remove(v);
            modHand.Insert(sec, v);
            modHand.Remove(Sv);
            modHand.Insert(fst, Sv);

            selected = null;
        }
        else
        {
            selected = s;
            selectPrefab.SetActive(true);
            selectPrefab.transform.position = s.transform.position;
        }
    }

    public void FimMudar()
    {
        if (Player1.myTurn)
        {
            Player1.cards = modHand;
        }
        else
        {
            Player2.cards = modHand;
        }
    }

    IEnumerator TheEnd()
    {
        yield return new WaitForSeconds(2);
        endScreen.GetComponentInChildren<TextMeshProUGUI>().text = EndText;
        endScreen.SetActive(true);
    }

    public void restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        
    }
}
