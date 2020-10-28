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
    public int numStartingCards=7;
    public float drawTimeStagger =0.1f;

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
        CardBartok tCB;
        for(int i = 0; i<numStartingCards; i++)
        {
            for(int j=0; j<4; j++)
            {
                tCB =Draw();
                tCB.timeStart = Time.time + drawTimeStagger*(i*4 +j);
                players[(j+1)%4].AddCArd(tCB);
            }
        }
        Invoke("DrawFirstTarget", drawTimeStagger*(numStartingCards*4+4));
    }
    public void DrawFirstTarget()
    {
        CardBartok tCB = MoveToTarget(Draw());
    }
    public CardBartok MoveToTarget(CardBartok tCB)
    {
        tCB.timeStart=0;
        tCB.MoveTo(layout.discardPile.pos+Vector3.back);
        tCB.state = CBState.toTarget;
        tCB.faceUp = true;
        tCB.SetSortingLayerName("10");
        tCB.eventualSortLayer = layout.target.layerName;
        if(targetCard!= null)
        {
            MoveToDiscard(targetCard);
        }
        targetCard = tCB;
        return tCB;
    }
    public CardBartok MoveToDiscard(CardBartok tCB)
    {
        tCB.state = CBState.discard;
        discardPile.Add(tCB);
        tCB.SetSortingLayerName(layout.discardPile.layerName);
        tCB.SetSortOrder(discardPile.Count*4);
        tCB.transform.localPosition = layout.discardPile.pos +Vector3.back/2;
        return(tCB);
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
