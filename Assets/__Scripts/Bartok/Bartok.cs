﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bartok : MonoBehaviour
{
    static public Bartok S;

    [Header("set in inspector")]
    public TextAsset deckXML;
    public TextAsset layoutXML;
    public Vector3 layoutCenter = Vector3.zero;
    public float handFanDegrees = 10f;

    [Header("set dynamically")]
    public Deck deck;
    public List<CardBartok> drawPile;
    public List<CardBartok> discardPile;
    public List<Player> players;
    public CardBartok targetCard;

    private BartokLayout layout;
    private Transform layoutAnchor;

    private void Awake()
    {
        S=this;
    }
    private void Start()
    {
        deck = GetComponent<Deck>();
        deck.InitDeck(deckXML.text);
        Deck.Shuffle(ref deck.cards);
        layout = GetComponent<BartokLayout>();
        layout.ReadLayout(layoutXML.text);
        drawPile = UpgradeCardsList(deck.cards);
        LayoutGame();

    }
    List<CardBartok>UpgradeCardsList(List<Card> lCD)
    {
        List<CardBartok> lCB = new List<CardBartok>();
        foreach(Card tCD in lCD)
        {
            lCB.Add(tCD as CardBartok);
        }
        return(lCB);
    }
    public void ArrangeDrawPile()
    {
        CardBartok tCB;
        for(int i =0; i<drawPile.Count; i++)
        {
            tCB =drawPile[i];
            tCB.transform.SetParent(layoutAnchor);
            tCB.transform.localPosition = layout.drawPile.pos;

            tCB.faceUp = false;
            tCB.SetSortingLayerName(layout.drawPile.layerName);
            tCB.SetSortOrder(-i*4);
            tCB.state = CBState.drawpile;

        }
    }
    void LayoutGame()
    {
        if(layoutAnchor==null)
        {
            GameObject tGO = new GameObject("_LayoutAnchor");
            layoutAnchor = tGO.transform;
            layoutAnchor.transform.position = layoutCenter;
        }

        ArrangeDrawPile();
        Player pl;
        players = new List<Player>();
        foreach(SlotDef tSD in layout.slotDefs)
        {
            pl = new Player();
            pl.handSlotDef = tSD;
            players.Add(pl);
            pl.playerNum = tSD.player;
        
        }
        players[0].type = PlayerType.human;
    }
    public CardBartok Draw()
    {
        CardBartok cd = drawPile[0];
        drawPile.RemoveAt(0);
        return cd;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            //print("hello");
            players[0].AddCArd(Draw());
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            players[1].AddCArd(Draw());
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            players[2].AddCArd(Draw());
        }
        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            players[3].AddCArd(Draw());
        }
    }
}
