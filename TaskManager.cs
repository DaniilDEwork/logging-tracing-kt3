using Serilog;

namespace TaskManagerApp;

public class TaskManager
{
    private readonly List<Task> tasks = new();
    private int nextId = 1;

    public void AddTask(string title)
    {
        var logger = Log.ForContext("Operation", "AddTask");
        logger.Debug("Начало операции AddTask. Название: {TaskTitle}", title);
        title = title.Trim();

        if (!IsValidTitle(title, logger))
        {
            logger.Debug("Конец операции AddTask. Результат: {Result}", "InvalidTitle");
            return;
        }

        var task = new Task { Id = nextId++, Title = title };
        tasks.Add(task);
        logger.ForContext("Result", "Success")
            .Information("Создана задача {@TaskItem}. Всего задач: {TaskCount}", task, tasks.Count);
        logger.Debug("Конец операции AddTask. Результат: {Result}", "Success");
    }

    public void RemoveTask(string title)
    {
        var logger = Log.ForContext("Operation", "RemoveTask");
        logger.Debug("Начало операции RemoveTask. Название: {TaskTitle}", title);
        title = title.Trim();

        if (!IsValidTitle(title, logger))
        {
            logger.Debug("Конец операции RemoveTask. Результат: {Result}", "InvalidTitle");
            return;
        }

        logger.Debug("Поиск задачи {TaskTitle} среди {TaskCount} задач", title, tasks.Count);
        int index = tasks.FindIndex(task =>
            string.Equals(task.Title, title, StringComparison.OrdinalIgnoreCase));

        if (index == -1)
        {
            logger.Error("Задача \"{TaskTitle:l}\" не найдена для удаления. Результат: {Result:l}. Всего задач: {TaskCount}",
                title, "NotFound", tasks.Count);
            logger.Debug("Конец операции RemoveTask. Результат: {Result}", "NotFound");
            return;
        }

        Task removedTask = tasks[index];
        tasks.RemoveAt(index);
        logger.ForContext("Result", "Success")
            .Information("Удалена задача {@TaskItem}. Всего задач: {TaskCount}", removedTask, tasks.Count);
        logger.Debug("Конец операции RemoveTask. Результат: {Result}", "Success");
    }

    public void ListTasks()
    {
        var logger = Log.ForContext("Operation", "ListTasks");
        logger.Debug("Начало операции ListTasks");

        if (tasks.Count == 0)
        {
            logger.Information("Список задач пуст");
        }
        else
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {tasks[i].Title}");
            }
        }

        logger.ForContext("Result", "Success")
            .Information("Текущий список задач: {@Tasks}. Всего задач: {TaskCount}", tasks, tasks.Count);
        logger.Debug("Конец операции ListTasks. Результат: {Result}", "Success");
    }

    private bool IsValidTitle(string title, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            logger.Warning("Пустое название задачи. Результат: {Result}. Название: {TaskTitle}. Всего задач: {TaskCount}",
                "InvalidTitle", title, tasks.Count);
            return false;
        }

        if (title.Length > 100 || title.Any(char.IsControl))
        {
            logger.Warning("Невалидное название: допустимо до 100 символов без управляющих символов. Результат: {Result}. Название: {TaskTitle}. Всего задач: {TaskCount}",
                "InvalidTitle", title, tasks.Count);
            return false;
        }

        return true;
    }
}
