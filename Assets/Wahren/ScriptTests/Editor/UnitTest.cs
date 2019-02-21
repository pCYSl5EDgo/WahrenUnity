using NUnit.Framework;
using pcysl5edgo.Wahren.AST;

using static UnityEngine.Assertions.Assert;
using static UnityEngine.Debug;

public unsafe class UnitTest
{
    [Test]
    public void movetype_consti_success_4()
    {
        var str0 = "o0_21uruse021902e";
        var strs = new[] { ("lawsuit", 7), ("death", 10), ("wind", 1), ("earth", 0) };
        var strx = strs[0].Item1 + "*" + strs[0].Item2.ToString();
        var times = "*";
        for (int i = 1; i < strs.Length; i++)
        {
            var (s, n) = strs[i];
            strx += ", " + s + times + n;
        }
        var scriptText = $@"movetype {str0}{{
            consti = {strx}
        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.MovetypeParserTempData.Length, 1);
            ref var tree = ref _.script.MovetypeParserTempData.Values[0];
            AreEqual(tree.Length, 1);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, tree.Name).ToString(), str0);
            AreEqual(tree.ParentName.Length, 0);
            var (value, type) = _.script.ASTValueTypePairList[tree.Start];
            AreEqual(value, 0);
            AreEqual((MovetypeTree.Kind)type, MovetypeTree.Kind.consti);
            AreEqual(_.script.MovetypeParserTempData.ConstiLength, 1);
            var expression = _.script.MovetypeParserTempData.Constis[value];
            AreEqual(expression.ScenarioVariant.Length, 0);
            AreEqual(expression.Start, 0);
            AreEqual(expression.Length, strs.Length);
            var pairs = _.script.MovetypeParserTempData.IdentifierNumberPairs;
            AreEqual(pairs.Length, strs.Length);
            for (int i = expression.Start, end = expression.Start + expression.Length; i < end; i++)
            {
                var p = pairs.Values[i];
                var (s, n) = strs[i - expression.Start];
                AreEqual(buffer.Clear().AppendPrimitive(_.file, p.Span).ToString(), s);
                AreEqual(p.Number, n);
            }
        }
    }
    [Test]
    public void movetype_consti_success_1()
    {
        var str0 = "o0_21uruse021902e";
        var strs = new[] { ("lawsuit", 7) };
        var strx = strs[0].Item1 + "*" + strs[0].Item2.ToString();
        var times = "*";
        for (int i = 1; i < strs.Length; i++)
        {
            var (s, n) = strs[i];
            strx += ", " + s + times + n;
        }
        var scriptText = $@"movetype {str0}{{
            consti = {strx}
        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.MovetypeParserTempData.Length, 1);
            ref var tree = ref _.script.MovetypeParserTempData.Values[0];
            AreEqual(tree.Length, 1);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, tree.Name).ToString(), str0);
            AreEqual(tree.ParentName.Length, 0);
            var (value, type) = _.script.ASTValueTypePairList[tree.Start];
            AreEqual(value, 0);
            AreEqual((MovetypeTree.Kind)type, MovetypeTree.Kind.consti);
            AreEqual(_.script.MovetypeParserTempData.ConstiLength, 1);
            var expression = _.script.MovetypeParserTempData.Constis[value];
            AreEqual(expression.ScenarioVariant.Length, 0);
            AreEqual(expression.Start, 0);
            AreEqual(expression.Length, 1);
            var pairs = _.script.MovetypeParserTempData.IdentifierNumberPairs;
            AreEqual(pairs.Length, 1);
            for (int i = expression.Start, end = expression.Start + expression.Length; i < end; i++)
            {
                var p = pairs.Values[i];
                var (s, n) = strs[i - expression.Start];
                AreEqual(buffer.Clear().AppendPrimitive(_.file, p.Span).ToString(), s);
                AreEqual(p.Number, n);
            }
        }
    }
    [Test]
    public void movetype_consti_success_0()
    {
        var str0 = "o0_21uruse021902e";
        var scriptText = $@"movetype {str0}{{
            consti = @
        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.MovetypeParserTempData.Length, 1);
            ref var tree = ref _.script.MovetypeParserTempData.Values[0];
            AreEqual(tree.Length, 1);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, tree.Name).ToString(), str0);
            AreEqual(tree.ParentName.Length, 0);
            var (value, type) = _.script.ASTValueTypePairList[tree.Start];
            AreEqual(value, 0);
            AreEqual((MovetypeTree.Kind)type, MovetypeTree.Kind.consti);
            AreEqual(_.script.MovetypeParserTempData.ConstiLength, 1);
            var expression = _.script.MovetypeParserTempData.Constis[value];
            AreEqual(expression.ScenarioVariant.Length, 0);
            AreEqual(expression.Start, 0);
            AreEqual(expression.Length, 0);
        }
    }
    [Test]
    public void movetype_consti_success_4_space()
    {
        var str0 = "o0_21uruse021902e";
        var strs = new[] { ("lawsuit", 7), ("death", 10), ("wind", 1), ("earth", 0) };
        var times = "  \t\t  *    \t\t\t";
        var strx = strs[0].Item1 + times + strs[0].Item2.ToString();
        for (int i = 1; i < strs.Length; i++)
        {
            var (s, n) = strs[i];
            strx += "," + s + times + n;
        }
        var scriptText = $@"movetype {str0}{{
            consti = {strx}
        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.MovetypeParserTempData.Length, 1);
            ref var tree = ref _.script.MovetypeParserTempData.Values[0];
            AreEqual(tree.Length, 1);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, tree.Name).ToString(), str0);
            AreEqual(tree.ParentName.Length, 0);
            var (value, type) = _.script.ASTValueTypePairList[tree.Start];
            AreEqual(value, 0);
            AreEqual((MovetypeTree.Kind)type, MovetypeTree.Kind.consti);
            AreEqual(_.script.MovetypeParserTempData.ConstiLength, 1);
            var expression = _.script.MovetypeParserTempData.Constis[value];
            AreEqual(expression.ScenarioVariant.Length, 0);
            AreEqual(expression.Start, 0);
            AreEqual(expression.Length, strs.Length);
            var pairs = _.script.MovetypeParserTempData.IdentifierNumberPairs;
            AreEqual(pairs.Length, strs.Length);
            for (int i = expression.Start, end = expression.Start + expression.Length; i < end; i++)
            {
                var p = pairs.Values[i];
                var (s, n) = strs[i - expression.Start];
                AreEqual(buffer.Clear().AppendPrimitive(_.file, p.Span).ToString(), s);
                AreEqual(p.Number, n);
            }
        }
    }
    [Test]
    public void movetype_consti_success_1_space()
    {
        var str0 = "o0_21uruse021902e";
        var strs = new[] { ("lawsuit", 7) };
        var times = "  \t\t  *    \t\t\t";
        var strx = strs[0].Item1 + times + strs[0].Item2.ToString();
        for (int i = 1; i < strs.Length; i++)
        {
            var (s, n) = strs[i];
            strx += ",\n\n\t" + s + times + n;
        }
        var scriptText = $@"movetype {str0}{{
            consti = {strx}
        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.MovetypeParserTempData.Length, 1);
            ref var tree = ref _.script.MovetypeParserTempData.Values[0];
            AreEqual(tree.Length, 1);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, tree.Name).ToString(), str0);
            AreEqual(tree.ParentName.Length, 0);
            var (value, type) = _.script.ASTValueTypePairList[tree.Start];
            AreEqual(value, 0);
            AreEqual((MovetypeTree.Kind)type, MovetypeTree.Kind.consti);
            AreEqual(_.script.MovetypeParserTempData.ConstiLength, 1);
            var expression = _.script.MovetypeParserTempData.Constis[value];
            AreEqual(expression.ScenarioVariant.Length, 0);
            AreEqual(expression.Start, 0);
            AreEqual(expression.Length, 1);
            var pairs = _.script.MovetypeParserTempData.IdentifierNumberPairs;
            AreEqual(pairs.Length, 1);
            for (int i = expression.Start, end = expression.Start + expression.Length; i < end; i++)
            {
                var p = pairs.Values[i];
                var (s, n) = strs[i - expression.Start];
                AreEqual(buffer.Clear().AppendPrimitive(_.file, p.Span).ToString(), s);
                AreEqual(p.Number, n);
            }
        }
    }
    [Test]
    public void movetype_consti_success_0_space()
    {
        var str0 = "o0_21uruse021902e";
        var scriptText = $@"movetype {str0}{{

            consti =      @


        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.MovetypeParserTempData.Length, 1);
            ref var tree = ref _.script.MovetypeParserTempData.Values[0];
            AreEqual(tree.Length, 1);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, tree.Name).ToString(), str0);
            AreEqual(tree.ParentName.Length, 0);
            var (value, type) = _.script.ASTValueTypePairList[tree.Start];
            AreEqual(value, 0);
            AreEqual((MovetypeTree.Kind)type, MovetypeTree.Kind.consti);
            AreEqual(_.script.MovetypeParserTempData.ConstiLength, 1);
            var expression = _.script.MovetypeParserTempData.Constis[value];
            AreEqual(expression.ScenarioVariant.Length, 0);
            AreEqual(expression.Start, 0);
            AreEqual(expression.Length, 0);
        }
    }
    [Test]
    public void movetype_help_success()
    {
        var str0 = "1x";
        var str1 = "好きだああああ！！！愛しているよぉおおおおおおお！！！";
        var scriptText = $@"movetype {str0}{{
            help = {str1}
        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.MovetypeParserTempData.Length, 1);
            ref var tree = ref _.script.MovetypeParserTempData.Values[0];
            AreEqual(tree.Length, 1);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, tree.Name).ToString(), str0);
            AreEqual(tree.ParentName.Length, 0);
            var (value, type) = _.script.ASTValueTypePairList[tree.Start];
            AreEqual(value, 0);
            AreEqual((MovetypeTree.Kind)type, MovetypeTree.Kind.help);
            AreEqual(_.script.MovetypeParserTempData.HelpLength, 1);
            var expression = _.script.MovetypeParserTempData.Helps[value];
            AreEqual(expression.ScenarioVariant.Length, 0);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, expression.Value).ToString(), str1);
        }
    }

    [Test]
    public void movetype_help_fail()
    {
        var str0 = "1145141919810_";
        var str1 = "絶対に許さねえ！\nドン・サウザンド！";
        var scriptText = $@"movetype {str0}{{
            help = {str1}
        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.MovetypeParserTempData.Length, 0);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, _.commonData.LastNameSpan).ToString(), str0);
            AreEqual(_.commonData.LastParentNameSpan.Length, 0);
            AreEqual(_.commonData.LastStructKind, Location.Movetype);
            AreEqual(_.commonData.Result.Status, InterpreterStatus.Error);
            AreEqual(_.commonData.Result.Span, new Span(0, 2, 0, 1));
        }
    }

    [Test]
    public void movetype_name_success()
    {
        var str0 = "1x";
        var str1 = "好きだああああ！！！愛しているよぉおおおおおおお！！！";
        var scriptText = $@"movetype {str0}{{
            name = {str1}
        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.MovetypeParserTempData.Length, 1);
            ref var tree = ref _.script.MovetypeParserTempData.Values[0];
            AreEqual(tree.Length, 1);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, tree.Name).ToString(), str0);
            AreEqual(tree.ParentName.Length, 0);
            var (value, type) = _.script.ASTValueTypePairList[tree.Start];
            AreEqual(value, 0);
            AreEqual((MovetypeTree.Kind)type, MovetypeTree.Kind.name);
            AreEqual(_.script.MovetypeParserTempData.NameLength, 1);
            var expression = _.script.MovetypeParserTempData.Names[value];
            AreEqual(expression.ScenarioVariant.Length, 0);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, expression.Value).ToString(), str1);
        }
    }

    [Test]
    public void movetype_name_fail()
    {
        var str0 = "1145141919810_";
        var str1 = "絶対に許さねえ！\nドン・サウザンド！";
        var scriptText = $@"movetype {str0}{{
            name = {str1}
        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.MovetypeParserTempData.Length, 0);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, _.commonData.LastNameSpan).ToString(), str0);
            AreEqual(_.commonData.LastParentNameSpan.Length, 0);
            AreEqual(_.commonData.LastStructKind, Location.Movetype);
            AreEqual(_.commonData.Result.Status, InterpreterStatus.Error);
            AreEqual(_.commonData.Result.Span, new Span(0, 2, 0, 1));
        }
    }

    [Test]
    public void race_consti_success_4()
    {
        var str0 = "o0_21uruse021902e";
        var strs = new[] { ("lawsuit", 7), ("death", 10), ("wind", 1), ("earth", 0) };
        var strx = strs[0].Item1 + "*" + strs[0].Item2.ToString();
        var times = "*";
        for (int i = 1; i < strs.Length; i++)
        {
            var (s, n) = strs[i];
            strx += ", " + s + times + n;
        }
        var scriptText = $@"race {str0}{{
            consti = {strx}
        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.RaceParserTempData.Length, 1);
            ref var tree = ref _.script.RaceParserTempData.Values[0];
            AreEqual(tree.Length, 1);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, tree.Name).ToString(), str0);
            AreEqual(tree.ParentName.Length, 0);
            var (value, type) = _.script.ASTValueTypePairList[tree.Start];
            AreEqual(value, 0);
            AreEqual((RaceTree.Kind)type, RaceTree.Kind.consti);
            AreEqual(_.script.RaceParserTempData.ConstiLength, 1);
            var expression = _.script.RaceParserTempData.Constis[value];
            AreEqual(expression.ScenarioVariant.Length, 0);
            AreEqual(expression.Start, 0);
            AreEqual(expression.Length, strs.Length);
            var pairs = _.script.RaceParserTempData.IdentifierNumberPairs;
            AreEqual(pairs.Length, strs.Length);
            for (int i = expression.Start, end = expression.Start + expression.Length; i < end; i++)
            {
                var p = pairs.Values[i];
                var (s, n) = strs[i - expression.Start];
                AreEqual(buffer.Clear().AppendPrimitive(_.file, p.Span).ToString(), s);
                AreEqual(p.Number, n);
            }
        }
    }
    [Test]
    public void race_consti_success_1()
    {
        var str0 = "o0_21uruse021902e";
        var strs = new[] { ("lawsuit", 7) };
        var strx = strs[0].Item1 + "*" + strs[0].Item2.ToString();
        var times = "*";
        for (int i = 1; i < strs.Length; i++)
        {
            var (s, n) = strs[i];
            strx += ", " + s + times + n;
        }
        var scriptText = $@"race {str0}{{
            consti = {strx}
        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.RaceParserTempData.Length, 1);
            ref var tree = ref _.script.RaceParserTempData.Values[0];
            AreEqual(tree.Length, 1);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, tree.Name).ToString(), str0);
            AreEqual(tree.ParentName.Length, 0);
            var (value, type) = _.script.ASTValueTypePairList[tree.Start];
            AreEqual(value, 0);
            AreEqual((RaceTree.Kind)type, RaceTree.Kind.consti);
            AreEqual(_.script.RaceParserTempData.ConstiLength, 1);
            var expression = _.script.RaceParserTempData.Constis[value];
            AreEqual(expression.ScenarioVariant.Length, 0);
            AreEqual(expression.Start, 0);
            AreEqual(expression.Length, 1);
            var pairs = _.script.RaceParserTempData.IdentifierNumberPairs;
            AreEqual(pairs.Length, 1);
            for (int i = expression.Start, end = expression.Start + expression.Length; i < end; i++)
            {
                var p = pairs.Values[i];
                var (s, n) = strs[i - expression.Start];
                AreEqual(buffer.Clear().AppendPrimitive(_.file, p.Span).ToString(), s);
                AreEqual(p.Number, n);
            }
        }
    }
    [Test]
    public void race_consti_success_0()
    {
        var str0 = "o0_21uruse021902e";
        var scriptText = $@"race {str0}{{
            consti = @
        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.RaceParserTempData.Length, 1);
            ref var tree = ref _.script.RaceParserTempData.Values[0];
            AreEqual(tree.Length, 1);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, tree.Name).ToString(), str0);
            AreEqual(tree.ParentName.Length, 0);
            var (value, type) = _.script.ASTValueTypePairList[tree.Start];
            AreEqual(value, 0);
            AreEqual((RaceTree.Kind)type, RaceTree.Kind.consti);
            AreEqual(_.script.RaceParserTempData.ConstiLength, 1);
            var expression = _.script.RaceParserTempData.Constis[value];
            AreEqual(expression.ScenarioVariant.Length, 0);
            AreEqual(expression.Start, 0);
            AreEqual(expression.Length, 0);
        }
    }
    [Test]
    public void race_consti_success_4_space()
    {
        var str0 = "o0_21uruse021902e";
        var strs = new[] { ("lawsuit", 7), ("death", 10), ("wind", 1), ("earth", 0) };
        var times = "  \t\t  *    \t\t\t";
        var strx = strs[0].Item1 + times + strs[0].Item2.ToString();
        for (int i = 1; i < strs.Length; i++)
        {
            var (s, n) = strs[i];
            strx += "," + s + times + n;
        }
        var scriptText = $@"race {str0}{{
            consti = {strx}
        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.RaceParserTempData.Length, 1);
            ref var tree = ref _.script.RaceParserTempData.Values[0];
            AreEqual(tree.Length, 1);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, tree.Name).ToString(), str0);
            AreEqual(tree.ParentName.Length, 0);
            var (value, type) = _.script.ASTValueTypePairList[tree.Start];
            AreEqual(value, 0);
            AreEqual((RaceTree.Kind)type, RaceTree.Kind.consti);
            AreEqual(_.script.RaceParserTempData.ConstiLength, 1);
            var expression = _.script.RaceParserTempData.Constis[value];
            AreEqual(expression.ScenarioVariant.Length, 0);
            AreEqual(expression.Start, 0);
            AreEqual(expression.Length, strs.Length);
            var pairs = _.script.RaceParserTempData.IdentifierNumberPairs;
            AreEqual(pairs.Length, strs.Length);
            for (int i = expression.Start, end = expression.Start + expression.Length; i < end; i++)
            {
                var p = pairs.Values[i];
                var (s, n) = strs[i - expression.Start];
                AreEqual(buffer.Clear().AppendPrimitive(_.file, p.Span).ToString(), s);
                AreEqual(p.Number, n);
            }
        }
    }
    [Test]
    public void race_consti_success_1_space()
    {
        var str0 = "o0_21uruse021902e";
        var strs = new[] { ("lawsuit", 7) };
        var times = "  \t\t  *    \t\t\t";
        var strx = strs[0].Item1 + times + strs[0].Item2.ToString();
        for (int i = 1; i < strs.Length; i++)
        {
            var (s, n) = strs[i];
            strx += ",\n\n\t" + s + times + n;
        }
        var scriptText = $@"race {str0}{{
            consti = {strx}
        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.RaceParserTempData.Length, 1);
            ref var tree = ref _.script.RaceParserTempData.Values[0];
            AreEqual(tree.Length, 1);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, tree.Name).ToString(), str0);
            AreEqual(tree.ParentName.Length, 0);
            var (value, type) = _.script.ASTValueTypePairList[tree.Start];
            AreEqual(value, 0);
            AreEqual((RaceTree.Kind)type, RaceTree.Kind.consti);
            AreEqual(_.script.RaceParserTempData.ConstiLength, 1);
            var expression = _.script.RaceParserTempData.Constis[value];
            AreEqual(expression.ScenarioVariant.Length, 0);
            AreEqual(expression.Start, 0);
            AreEqual(expression.Length, 1);
            var pairs = _.script.RaceParserTempData.IdentifierNumberPairs;
            AreEqual(pairs.Length, 1);
            for (int i = expression.Start, end = expression.Start + expression.Length; i < end; i++)
            {
                var p = pairs.Values[i];
                var (s, n) = strs[i - expression.Start];
                AreEqual(buffer.Clear().AppendPrimitive(_.file, p.Span).ToString(), s);
                AreEqual(p.Number, n);
            }
        }
    }
    [Test]
    public void race_consti_success_0_space()
    {
        var str0 = "o0_21uruse021902e";
        var scriptText = $@"race {str0}{{

            consti =      @


        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.RaceParserTempData.Length, 1);
            ref var tree = ref _.script.RaceParserTempData.Values[0];
            AreEqual(tree.Length, 1);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, tree.Name).ToString(), str0);
            AreEqual(tree.ParentName.Length, 0);
            var (value, type) = _.script.ASTValueTypePairList[tree.Start];
            AreEqual(value, 0);
            AreEqual((RaceTree.Kind)type, RaceTree.Kind.consti);
            AreEqual(_.script.RaceParserTempData.ConstiLength, 1);
            var expression = _.script.RaceParserTempData.Constis[value];
            AreEqual(expression.ScenarioVariant.Length, 0);
            AreEqual(expression.Start, 0);
            AreEqual(expression.Length, 0);
        }
    }
    [Test]
    public void race_movetype_fail()
    {
        var str0 = "o0_21uruse021902e";
        var str1 = "絶対に許早苗！！！！！！";
        var scriptText = $@"race {str0}{{
            movetype = {str1}
        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(buffer.Clear().AppendPrimitive(_.file, _.commonData.LastNameSpan).ToString(), str0);
            AreEqual(_.commonData.LastParentNameSpan.Length, 0);
            AreEqual(_.commonData.LastStructKind, Location.Race);
            AreEqual((ErrorSentence.Kind)_.commonData.Result.DataIndex, ErrorSentence.Kind.InvalidIdentifierError);
            AreEqual(_.commonData.Result.Status, InterpreterStatus.Error);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, _.commonData.Result.Span).ToString(), str1[0].ToString());
        }
    }

    [Test]
    public void race_movetype_success_1()
    {
        var str0 = "o0_21uruse021902e";
        var str1 = "jitunikitanaihatugenwomatikamaeteitahitobito";
        var scriptText = $@"race {str0}{{
            movetype = {str1}
        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.RaceParserTempData.Length, 1);
            ref var tree = ref _.script.RaceParserTempData.Values[0];
            AreEqual(tree.Length, 1);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, tree.Name).ToString(), str0);
            AreEqual(tree.ParentName.Length, 0);
            var (value, type) = _.script.ASTValueTypePairList[tree.Start];
            AreEqual(value, 0);
            AreEqual((RaceTree.Kind)type, RaceTree.Kind.movetype);
            AreEqual(_.script.RaceParserTempData.MovetypeLength, 1);
            var expression = _.script.RaceParserTempData.Movetypes[value];
            AreEqual(expression.ScenarioVariant.Length, 0);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, expression.Value).ToString(), str1);
        }
    }
    [Test]
    public void race_movetype_success_0()
    {
        var str0 = "o0_21uruse021902e";
        var scriptText = $@"race {str0}{{
            movetype = @
        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.RaceParserTempData.Length, 1);
            ref var tree = ref _.script.RaceParserTempData.Values[0];
            AreEqual(tree.Length, 1);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, tree.Name).ToString(), str0);
            AreEqual(tree.ParentName.Length, 0);
            var (value, type) = _.script.ASTValueTypePairList[tree.Start];
            AreEqual(value, 0);
            AreEqual((RaceTree.Kind)type, RaceTree.Kind.movetype);
            AreEqual(_.script.RaceParserTempData.MovetypeLength, 1);
            var expression = _.script.RaceParserTempData.Movetypes[value];
            AreEqual(expression.ScenarioVariant.Length, 0);
            AreEqual(expression.Value, new Span(0, 1, 23, 0));
        }
    }
    [Test]
    public void race_brave_success()
    {
        var str0 = "o0_21uruse021902e";
        var num0 = 100;
        var scriptText = $@"race {str0}{{
            brave = {num0}
        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.RaceParserTempData.Length, 1);
            ref var tree = ref _.script.RaceParserTempData.Values[0];
            AreEqual(tree.Length, 1);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, tree.Name).ToString(), str0);
            AreEqual(tree.ParentName.Length, 0);
            var (value, type) = _.script.ASTValueTypePairList[tree.Start];
            AreEqual(value, 0);
            AreEqual((RaceTree.Kind)type, RaceTree.Kind.brave);
            AreEqual(_.script.RaceParserTempData.BraveLength, 1);
            var expression = _.script.RaceParserTempData.Braves[value];
            AreEqual(expression.ScenarioVariant.Length, 0);
            AreEqual(expression.Value, num0);
        }
    }
    [Test]
    public void race_align_fail_lessthan0()
    {
        var str0 = "o0_21uruse021902e";
        var num0 = -1;
        var scriptText = $@"race {str0}{{
            brave = {num0}
        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.RaceParserTempData.Length, 0);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, _.commonData.LastNameSpan).ToString(), str0);
            AreEqual(_.commonData.LastParentNameSpan.Length, 0);
            AreEqual(_.commonData.LastStructKind, Location.Race);
            AreEqual((ErrorSentence.Kind)_.commonData.Result.DataIndex, ErrorSentence.Kind.OutOfRangeError);
            AreEqual(_.commonData.Result.Status, InterpreterStatus.Error);
            AreEqual(_.commonData.Result.Span, new Span(0, 1, 20, 2));
        }
    }
    [Test]
    public void race_brave_fail_greaterthan100()
    {
        var str0 = "o0_21uruse021902e";
        var num0 = 1000;
        var scriptText = $@"race {str0}{{
            brave = {num0}
        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.RaceParserTempData.Length, 0);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, _.commonData.LastNameSpan).ToString(), str0);
            AreEqual(_.commonData.LastParentNameSpan.Length, 0);
            AreEqual(_.commonData.LastStructKind, Location.Race);
            AreEqual((ErrorSentence.Kind)_.commonData.Result.DataIndex, ErrorSentence.Kind.OutOfRangeError);
            AreEqual(_.commonData.Result.Status, InterpreterStatus.Error);
            AreEqual(_.commonData.Result.Span, new Span(0, 1, 20, 4));
        }
    }
    [Test]
    public void race_brave_fail_lessthan0()
    {
        var str0 = "o0_21uruse021902e";
        var num0 = -1;
        var scriptText = $@"race {str0}{{
            align = {num0}
        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.RaceParserTempData.Length, 0);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, _.commonData.LastNameSpan).ToString(), str0);
            AreEqual(_.commonData.LastParentNameSpan.Length, 0);
            AreEqual(_.commonData.LastStructKind, Location.Race);
            AreEqual((ErrorSentence.Kind)_.commonData.Result.DataIndex, ErrorSentence.Kind.OutOfRangeError);
            AreEqual(_.commonData.Result.Status, InterpreterStatus.Error);
            AreEqual(_.commonData.Result.Span, new Span(0, 1, 20, 2));
        }
    }
    [Test]
    public void race_align_fail_greaterthan100()
    {
        var str0 = "o0_21uruse021902e";
        var num0 = 94218747;
        var scriptText = $@"race {str0}{{
            align = {num0}
        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.RaceParserTempData.Length, 0);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, _.commonData.LastNameSpan).ToString(), str0);
            AreEqual(_.commonData.LastParentNameSpan.Length, 0);
            AreEqual(_.commonData.LastStructKind, Location.Race);
            AreEqual((ErrorSentence.Kind)_.commonData.Result.DataIndex, ErrorSentence.Kind.OutOfRangeError);
            AreEqual(_.commonData.Result.Status, InterpreterStatus.Error);
            AreEqual(_.commonData.Result.Span, new Span(0, 1, 20, 8));
        }
    }
    [Test]
    public void race_align_success()
    {
        var str0 = "o0_21uruse021902e";
        var num0 = 100;
        var scriptText = $@"race {str0}{{
            align = {num0}
        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.RaceParserTempData.Length, 1);
            ref var tree = ref _.script.RaceParserTempData.Values[0];
            AreEqual(tree.Length, 1);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, tree.Name).ToString(), str0);
            AreEqual(tree.ParentName.Length, 0);
            var (value, type) = _.script.ASTValueTypePairList[tree.Start];
            AreEqual(value, 0);
            AreEqual((RaceTree.Kind)type, RaceTree.Kind.align);
            AreEqual(_.script.RaceParserTempData.AlignLength, 1);
            var expression = _.script.RaceParserTempData.Aligns[value];
            AreEqual(expression.ScenarioVariant.Length, 0);
            AreEqual(expression.Value, num0);
        }
    }
    [Test]
    public void race_name_success()
    {
        var str0 = "o0_21uruse021902e";
        var str1 = "絶対に許さねえ！ドン・サウザンド！";
        var scriptText = $@"race {str0}{{
            name = {str1}
        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.RaceParserTempData.Length, 1);
            ref var tree = ref _.script.RaceParserTempData.Values[0];
            AreEqual(tree.Length, 1);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, tree.Name).ToString(), str0);
            AreEqual(tree.ParentName.Length, 0);
            var (value, type) = _.script.ASTValueTypePairList[tree.Start];
            AreEqual(value, 0);
            AreEqual((RaceTree.Kind)type, RaceTree.Kind.name);
            AreEqual(_.script.RaceParserTempData.NameLength, 1);
            var expression = _.script.RaceParserTempData.Names[value];
            AreEqual(expression.ScenarioVariant.Length, 0);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, expression.Value).ToString(), str1);
        }
    }

    [Test]
    public void race_name_fail()
    {
        var str0 = "o0_21uruse021902e";
        var str1 = "絶対に許さねえ！\nドン・サウザンド！";
        var scriptText = $@"race {str0}{{
            name = {str1}
        }}";
        using (var _ = new USING_STRUCT(scriptText, out var buffer))
        {
            AreEqual(_.script.RaceParserTempData.Length, 0);
            AreEqual(buffer.Clear().AppendPrimitive(_.file, _.commonData.LastNameSpan).ToString(), str0);
            AreEqual(_.commonData.LastParentNameSpan.Length, 0);
            AreEqual(_.commonData.LastStructKind, Location.Race);
            AreEqual(_.commonData.Result.Status, InterpreterStatus.Error);
            AreEqual(_.commonData.Result.Span, new Span(0, 2, 0, 1));
        }
    }
}