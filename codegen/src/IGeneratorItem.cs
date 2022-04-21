namespace CodeGen
{
    public interface IGeneratorItem
    {
        void Generate(GeneratorWriter writer);
    }
}