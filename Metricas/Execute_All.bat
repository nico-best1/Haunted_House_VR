@echo off
echo Activando entorno virtual...

call venv\Scripts\activate

echo Ejecutando script stress_metrics.py...
python src\stress_metrics.py
echo.
echo Ejecucion stress_metrics finalizada.

echo Ejecutando script spatial_heatmap.py...
python src\spatial_heatmap.py
echo.
echo Ejecucion spatial_heatmap finalizada.

pause
