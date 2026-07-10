using UnityEngine;

public class SymbolPuzzlePart : BombPart
{
    [SerializeField] private Symbol[] symbols;
    [SerializeField] private int[] code;
    bool overcharged = true;
    bool electricity = false;

    [SerializeField] private float overTime;
    [SerializeField] private float veryOverTime;
    private float overCounter;

    public override bool OnItemUsed(ItemActionType type)
    {
        int elementID = 0;

        if (!UseBase(symbols, type, ref elementID))
        {
            return false;
        }

        symbols[elementID].anim.SetTrigger("Click");
        symbols[elementID].icon.NextColor();
        symbols[elementID].RemoveHighlight();

        bool solved = true;

        for (int i = 0; i < symbols.Length; i++)
        {
            if (symbols[i].icon.current != code[i])
            {
                solved = false;
                break;
            }
        }

        if (solved)
        {
            Solve();
        }
        return true;
    }

    private void Update()
    {
        if (!overcharged)
        {
            return;
        }

        if (overCounter <= 0)
        {
            overCounter = overTime;
            while (true)
            {
                int r = Random.Range(0, symbols.Length);
                symbols[r].icon.NextColor();

                bool solved = true;

                for (int i = 0; i < symbols.Length; i++)
                {
                    if (symbols[i].icon.current != code[i])
                    {
                        solved = false;
                        break;
                    }
                }

                if (!solved)
                {
                    break;
                }
            }
        }
        else
        {
            overCounter -= Time.deltaTime;
        }

    }

    public void DisableOvercharged()
    {
        overcharged = false;
        if (electricity)
        {
            Unlock();
            //just work, lights have colors
        }
        else
        {

            for (int i = 0; i < symbols.Length; i++)
            {
                symbols[i].icon.disabled = true;
                symbols[i].icon.NextColor();
            }
        }
    }

    public void EnableElectricity()
    {
        electricity = true;
        if (!overcharged)
        {
            while (true)
            {
                for (int i = 0; i < symbols.Length; i++)
                {
                    symbols[i].icon.disabled = false;
                    symbols[i].icon.NextColor();
                }

                bool solved = true;

                for (int i = 0; i < symbols.Length; i++)
                {
                    if (symbols[i].icon.current != code[i])
                    {
                        solved = false;
                        break;
                    }
                }

                if (!solved)
                {
                    break;
                }
            }
            Unlock();
        }
        else
        {
            overTime = veryOverTime;
        }
    }

}
