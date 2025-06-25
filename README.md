# RoverMissionPlanner

Microservicio .NET 8 para planificar y visualizar tareas de rovers en Marte.

## Estructura
- **Domain**: Entidades y lógica de dominio
- **Application**: Casos de uso y lógica de aplicación
- **Infrastructure**: Persistencia y servicios externos (en memoria por defecto)
- **API**: Endpoints REST
- **Tests**: Pruebas unitarias (xUnit)

## Instalación y uso

1. **Restaurar dependencias y compilar:**
   ```sh
   dotnet build
   ```
2. **Ejecutar la API:**
   ```sh
   dotnet run --project RoverMissionPlanner.API
   ```
3. **Ejecutar pruebas:**
   ```sh
   dotnet test
   ```

## Notas
- Requiere .NET 8 SDK o superior.
- Persistencia en memoria (puedes migrar a SQLite/LiteDB fácilmente).
- Validaciones con FluentValidation.
- Cobertura de pruebas >70% en lógica de solapamiento.
- Preparado para agregar SPA Angular 17+.
