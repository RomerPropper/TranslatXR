FROM tiangolo/uvicorn-gunicorn-fastapi:python3.11

COPY ./requirements-prod.txt /app/requirements.txt

COPY ./server_prod.py /app/main.py

RUN pip install --no-cache-dir --upgrade -r /app/requirements.txt
RUN pip install torch --index-url https://download.pytorch.org/whl/cpu
