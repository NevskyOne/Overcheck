using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class DialogParser : MonoBehaviour
{
    public TextAsset sourceText;
    public static List<List<DialogFragment>> ParsedFragments = new ();
    public static List<DialogFragment> PassFragmenst = new ();
    public static List<DialogFragment> NotPassFragmenst = new ();

    void Start()
    {
        var dialogs = sourceText.text.Split('/');
        foreach (var dialog in dialogs)
        {
            var mainText = dialog.Split("(Фраза пропуска)")[0];
            ParsedFragments.Add(ParseDialog(mainText));
            var ending = dialog.Split("(Фраза пропуска)")[1];
            PassFragmenst.Add(ParseDialog(ending.Split("(Фраза не допуска)")[0])[0]);
            NotPassFragmenst.Add(ParseDialog(ending.Split("(Фраза не допуска)")[1])[0]);
        }
    }

    List<DialogFragment> ParseDialog(string text)
    {
        string[] lines = text.Split('\n').Select(l => l.TrimEnd('\r')).ToArray();
        // Стек, где каждый элемент - список фрагментов текущего уровня вложенности.
        Stack<List<DialogFragment>> fragmentStack = new Stack<List<DialogFragment>>();
        // Начальный уровень — корневой список
        fragmentStack.Push(new List<DialogFragment>());

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();
            if (string.IsNullOrEmpty(line))
                continue; // пропускаем пустые строки

            // Определяем уровень вложенности по количеству '>'
            int level = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '>')
                    level++;
                else
                    break;
            }

            string content = line.Substring(level).Trim();

            bool isButton = (content.StartsWith("[") && content.EndsWith("]"));
            bool isGiveDocs = content.Contains("{Выдать документы}");
            if (isGiveDocs)
            {
                content = content.Replace("{Выдать документы}", "").Trim();
            }

            // Если текущая глубина стека больше чем level+1 — поднимаемся выше
            while (fragmentStack.Count > level + 1)
            {
                fragmentStack.Pop();
            }
            

            var currentLevelFragments = fragmentStack.Peek();

            if (isButton)
            {
                // Это кнопка
                string buttonText = content.Substring(1, content.Length - 2).Trim();

                // Нужно добавить кнопку к последнему фрагменту текущего уровня.
                // Если фрагментов нет, создадим пустой фрагмент
                if (currentLevelFragments.Count == 0)
                {
                    DialogFragment newFrag = new DialogFragment {
                        Text = "",
                        GiveDocs = false,
                        Buttons = new List<ButtonSt>()
                    };
                    currentLevelFragments.Add(newFrag);
                }

                // Добавляем кнопку к последнему фрагменту
                int lastFragIndex = currentLevelFragments.Count - 1;
                DialogFragment lastFrag = currentLevelFragments[lastFragIndex];
                if (lastFrag.Buttons == null)
                    lastFrag.Buttons = new List<ButtonSt>();
                    

                ButtonSt newButton = new ButtonSt
                {
                    Text = buttonText,
                    Fragments = new List<DialogFragment>()
                };
                lastFrag.Buttons.Add(newButton);
                currentLevelFragments[lastFragIndex] = lastFrag;

                // Теперь все следующие строки на уровень глубже относятся к этой кнопке
                // Проверим, если мы уже не на нужном уровне:
                // Уровень кнопки это 'level', значит её фрагменты пойдут на 'level+1'
                // Создаём новый уровень для вложенных фрагментов кнопки:
                while (fragmentStack.Count > level + 1)
                {
                    fragmentStack.Pop();
                }
                fragmentStack.Push(newButton.Fragments);

            }
            else
            {
                // Обычный текстовый фрагмент
                DialogFragment newFrag = new DialogFragment
                {
                    Text = content,
                    GiveDocs = isGiveDocs,
                    Buttons = new List<ButtonSt>()
                };
                currentLevelFragments.Add(newFrag);
            }
        }

        // Поднимаемся к корню
        while (fragmentStack.Count > 1)
        {
            fragmentStack.Pop();
        }

        return fragmentStack.Pop();
    }
}
